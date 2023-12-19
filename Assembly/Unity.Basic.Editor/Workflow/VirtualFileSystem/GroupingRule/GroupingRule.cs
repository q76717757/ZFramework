using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace ZFramework.Editor
{
    /// <summary>
    /// 分包策略
    /// </summary>
    public class GroupingRule
    {
        //分割出来的所有块
        Dictionary<string, BundleBlock> blocks = new Dictionary<string, BundleBlock>();
        //合并出来的包
        Dictionary<string, BundleReference> bundles = new Dictionary<string, BundleReference>();

        public GroupingRule(List<AssetReference> allAssets)
        {
            SplitBlock(allAssets);
            List<BundleBlock> blocks = IgnoreAloneBlocks();
            CombineBlock(blocks);
            DependenceAnalysis();
        }

        //1.先分块
        void SplitBlock(List<AssetReference> allAssets)
        {
            foreach (AssetReference vfs in allAssets)
            {
                if (vfs.Type == AssetType.PublicAsset)
                {
                    if (!blocks.TryGetValue(vfs.Guid, out BundleBlock box))
                    {
                        box = new BundleBlock(vfs);
                        blocks.Add(vfs.Guid, box);
                    }
                    box.Root.Add(box);
                    SplitBlock(box, box, vfs);
                }
            }
        }
        void SplitBlock(BundleBlock root, BundleBlock local, AssetReference asset)
        {
            foreach (AssetReference child in asset.Children)
            {
                switch (child.Type)
                {
                    case AssetType.PublicAsset:
                    case AssetType.ShareAsset:
                        {
                            if (!blocks.TryGetValue(child.Guid, out BundleBlock childBox))
                            {
                                childBox = new BundleBlock(child);
                                blocks.Add(child.Guid, childBox);
                            }
                            //添加起源
                            childBox.Root.Add(root);
                            if (local.Childs.Add(childBox))
                            {
                                SplitBlock(root, childBox, child);
                            }
                        }
                        break;
                    case AssetType.AloneAsset:
                        SplitBlock(root, local, child);
                        break;
                }
            }
        }

        //2.排除掉被唯一依赖的块  这种块可以向上合并 并且不需要在AB包中公开
        List<BundleBlock> IgnoreAloneBlocks()//排除掉只被一个块依赖的块  这种块可以向上合并
        {
            List<BundleBlock> activeBlocks = new List<BundleBlock>();
            foreach (BundleBlock block in blocks.Values)
            {
                if (block.MainAsset.Type == AssetType.ShareAsset && block.Root.Count == 1)
                {
                }
                else
                {
                    activeBlocks.Add(block);
                }
            }
            return activeBlocks;
        }

        //3.合并块 形成包
        void CombineBlock(List<BundleBlock> blocks)
        {
            foreach (BundleBlock block in blocks)
            {
                //来自相同起源组合的块 合并成一个包
                IEnumerable<string> guids = block.Root.Select(x => x.MainAsset.Guid);
                string hash = ComputeBundleName(guids.ToList());

                if (!bundles.TryGetValue(hash, out BundleReference bundle))
                {
                    string bundleName = hash;
                    bundle = new BundleReference(bundleName);
                    bundles.Add(hash, bundle);
                }
                bundle.Blocks.Add(block);
                block.Bundle = bundle;
            }
        }
        string ComputeBundleName(List<string> guids)
        {
            //将块的主资源GUID->简单排序->首尾拼接->hash->作为包的名字
            guids.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));

            StringBuilder sb = new StringBuilder();
            foreach (string guid in guids)
            {
                sb.Append(guid);
            }
            var md5 = MD5Helper.BytesMD5(Encoding.ASCII.GetBytes(sb.ToString()));
            return md5;
        }

        //4.构建包之间的依赖关系
        void DependenceAnalysis()
        {
            foreach (BundleReference bundle in bundles.Values)//所有的包
            {
                //包中的块  所依赖的所有块所属的包  就是我的依赖包
                foreach (BundleBlock myBlock in bundle.Blocks)//包的所有块
                {
                    foreach (BundleBlock childBlock in myBlock.Childs)//每一块的依赖
                    {
                        if (childBlock.Bundle != bundle)//是本包的跳过
                        {
                            if (childBlock.Bundle != null)//独立块不在包里面 会遍历出独立块 独立块是没有所属包的 隐式合并
                            {
                                bundle.Children.Add(childBlock.Bundle);
                            }
                        }
                    }
                }
            }
        }

        //5.输出清单
        public BundleManifest GetManifest()
        {
            BundleManifest output = new BundleManifest();
            foreach (BundleReference bundle in bundles.Values)
            {
                BundleInfo bi = bundle.BuildInfo();
                output.AddBundleInfo(bi);
            }
            return output;
        }
    }
}
