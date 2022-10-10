using UnityEditor;

public class Bundler
{
    [MenuItem("Build/New Bundle")]
    static void BuildBundle()
    {
        BuildPipeline.BuildAssetBundles("./AssetBundles/", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Build/Append Bundle")]
    static void AppendBundle()
    {
        BuildPipeline.BuildAssetBundles("./AssetBundles/", BuildAssetBundleOptions.AppendHashToAssetBundleName, BuildTarget.StandaloneWindows);
    }
}
