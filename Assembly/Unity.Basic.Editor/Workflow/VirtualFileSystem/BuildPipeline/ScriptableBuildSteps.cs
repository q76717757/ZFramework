#if ENABLE_SBP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Tasks;

namespace ZFramework.Editor
{
    public static class ScriptableBuildSteps
    {
        //SBP的默认构建步骤
        public static IList<IBuildTask> Create(bool shaderTask, bool monoscriptTask)
        {
            List<IBuildTask> list = new List<IBuildTask>();
            list.Add(new SwitchToBuildPlatform());
            list.Add(new RebuildSpriteAtlasCache());
            list.Add(new BuildPlayerScripts());
            list.Add(new PostScriptsCallback());
            list.Add(new CalculateSceneDependencyData());
            list.Add(new CalculateCustomDependencyData());
            list.Add(new CalculateAssetDependencyData());
            list.Add(new StripUnusedSpriteSources());
            if (shaderTask)
            {
                list.Add(new CreateBuiltInShadersBundle("UnityBuiltInShaders.ab"));
            }
            if (monoscriptTask)
            {

                list.Add(new CreateMonoScriptBundle("UnityMonoScripts.ab"));
            }
            list.Add(new PostDependencyCallback());
            list.Add(new GenerateBundlePacking());
            if (shaderTask || monoscriptTask)
            {
                list.Add(new UpdateBundleObjectLayout());
            }
            list.Add(new GenerateBundleCommands());
            list.Add(new GenerateSubAssetPathMaps());
            list.Add(new GenerateBundleMaps());
            list.Add(new PostPackingCallback());
            list.Add(new WriteSerializedFiles());
            list.Add(new ArchiveAndCompressBundles());
            list.Add(new AppendBundleHash());
            list.Add(new GenerateLinkXml());
            list.Add(new PostWritingCallback());
            return list;
        }
    }

}
#endif