﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BundleSystem
{
    [System.Serializable]
    public class AssetbundleBuildManifest
    {
        public static bool TryParse(string json, out AssetbundleBuildManifest manifest)
        {
            if (string.IsNullOrEmpty(json))
            {
                manifest = default;
                return false;
            }

            try
            {
                manifest = JsonUtility.FromJson<AssetbundleBuildManifest>(json);
                return true;
            }
            catch 
            {
                manifest = default;
                return false;
            }
        }

        [System.Serializable]
        public struct BundleInfo
        {
            public CachedAssetBundle AsCached => new CachedAssetBundle(BundleName, Hash);
            public string BundleName;

            [SerializeField]
            string hashString_;
            public Hash128 Hash
            {
                get => Hash128.Parse(hashString_);
                set => hashString_ = value.ToString();
            }

            public List<string> Dependencies;
            public long Size;
        }

        public List<BundleInfo> BundleInfos = new List<BundleInfo>();
        public string BuildTarget;

        /// <summary>
        /// This does not included in hash calculation, used to find newer version between cached manifest and local manifest
        /// </summary>
        public long BuildTime;
        public string RemoteURL;

        [SerializeField]
        string globalHash_;
        public Hash128 GlobalHash
        {
            get => Hash128.Parse(globalHash_);
            set => globalHash_ = value.ToString();
        }

        public bool TryGetBundleInfo(string name, out BundleInfo info)
        {
            var index = BundleInfos.FindIndex(bundleInfo => bundleInfo.BundleName == name);
            info = index >= 0 ? BundleInfos[index] : default;
            return index >= 0;
        }

        public bool TryGetBundleHash(string name, out Hash128 hash)
        {
            if (TryGetBundleInfo(name, out var info))
            {
                hash = info.Hash;
                return true;
            }
            else
            {
                hash = default;
                return false;
            }
        }
    }
}
