using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

#if ENABLE_SBP
using UnityEditor.Build.Pipeline.Tasks;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine.Build.Pipeline;
#endif

namespace ZFramework.Editor
{
    public class ScriptableBuildPipeline
    {
#if ENABLE_SBP//可编程管线

        public BuildResult BuildAssetBundles(BuildParameters parameters, AssetBundleBuild[] bundleBuilds)
        {
            var b = new BundleBuildParameters(parameters.Target, parameters.Group, parameters.OutputFolder);
            var taskss = ScriptableBuildSteps.Create(true, false);
            ContentPipeline.BuildAssetBundles(b, new BundleBuildContent(bundleBuilds), out IBundleBuildResults a, taskss);//可编程管线构建
            return new BuildResult();
        }
#else//默认管线
        public BuildResult BuildAssetBundles(BuildParameters parameters, AssetBundleBuild[] bundleBuilds)
        {
            AssetBundleManifest a =  BuildPipeline.BuildAssetBundles(parameters.OutputFolder, bundleBuilds, BuildAssetBundleOptions.None, parameters.Target);
            return new BuildResult();
        }
#endif
    }
}
