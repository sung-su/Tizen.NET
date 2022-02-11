﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text.RegularExpressions;
using System.Xml.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Samsung.Tizen;

namespace Samsung.Tizen.Build.Tasks
{
    public class CheckApiVersion : Task
    {
        [Required]
        public string ManifestApiVersion { get; set; }

        [Required]
        public string TargetFrameWork { get; set; }

        [Required]
        public string TargetFrameworkVersion { get; set; }

        [Required]
        public string TargetPlatformVersion { get; set; }

        [Required]
        public ITaskItem[] ApiVersionList { get; set; }

        string ApiVersion { get; set; }

        public override bool Execute()
        {
            if (string.IsNullOrEmpty(ManifestApiVersion))
            {
                Log.LogError("ManifestApiVersion is Invalid {0}", ManifestApiVersion);
                return false;
            }

            if (TargetFrameWork.StartsWith("tizen"))
            {
                //net5.0 tfm, get api version from tpv
                if (string.IsNullOrEmpty(TargetFrameworkVersion))
                {
                    Log.LogError("TargetFrameworkVersion is Invalid {0}", TargetFrameworkVersion);
                    return false;
                }

                //convert tfv to api version
                foreach(var item in ApiVersionList)
                {
                    if (Regex.IsMatch(item.ItemSpec, TargetFrameworkVersion))
                    {
                        ApiVersion = item.GetMetadata("Value");
                        break;
                    }
                }
            }
            else if (TargetFrameWork.StartsWith("net6.0-tizen"))
            {
                //net6.0 tfm, get api version from tpv
                if (string.IsNullOrEmpty(TargetPlatformVersion))
                {
                    Log.LogError("TargetPlatformVersion is Invalid {0}", TargetPlatformVersion);
                    return false;
                }
                ApiVersion = TargetPlatformVersion;
            }

            if (string.Compare(ManifestApiVersion, ApiVersion.ToString()) > 0)
            {
                Log.LogError("Api version is Invalid {0}", ApiVersion);
                return false;
            }

            return true;
        }
    }
}
