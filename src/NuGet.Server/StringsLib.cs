// #if VS15 // NET452

namespace NuGet.Server.Infrastructure
{
    public class Start
    {
        public static void Main()
        {
               // Library or Console ??
        }
    }

    public class StringsLib
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        //[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal StringsLib()
        {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuGet.Server.Strings", typeof(StringsLib).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Package {0} already exists. The server is configured to not allow overwriting packages that already exist..
        /// </summary>
        internal static string Error_PackageAlreadyExists {
            get {
                return ResourceManager.GetString("Error_PackageAlreadyExists", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Package {0} is a symbols package (it contains .pdb files and a /src folder). The server is configured to ignore symbols packages..
        /// </summary>
        internal static string Error_SymbolsPackagesIgnored {
            get {
                return ResourceManager.GetString("Error_SymbolsPackagesIgnored", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The &apos;packages&apos; and &apos;versionConstraints&apos; parameters must have the same number of elements..
        /// </summary>
        internal static string GetUpdatesParameterMismatch {
            get {
                return ResourceManager.GetString("GetUpdatesParameterMismatch", resourceCulture);
            }
        }
    }

}

// #endif