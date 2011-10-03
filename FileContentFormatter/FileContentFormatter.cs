using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;
using ScrewTurn.Wiki;
using System.Linq;
using System.IO;
using Keeper.Garrett.ScrewTurn.Utility;
using System.Web;

namespace Keeper.Garrett.ScrewTurn.FileContentFormatter
{
    public class FileContentFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        //Tag format {FileCont args}
        private static readonly Regex TagRegex = new Regex(@"\{FileCont(?<arguments>(.*?))\}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        #region Lookup tables and lists
        private static readonly List<string> m_MediaPlayerTypes = new List<string>()
        {
            ".aif",
            ".aifc",
            ".aiff",
            ".asf",
            ".asx",
            ".au",
            ".avi",
            ".cda",
            ".dvr-ms",
            ".ivf",
            ".m1v",
            ".m3u",
            ".mid",
            ".midi",
            ".mov",
            ".mp2",
            ".mp3",
            ".mpa",
            ".mpe",
            ".mpeg",
            ".mpg",
            ".mpv2",
            ".qt",
            ".rmi",
            ".snd",
            ".vob",
            ".wav",
            ".wax",
            ".wm",
            ".wma",
            ".wms",
            ".wmv",
            ".wmx",
            ".wmz",
            ".wpl",
            ".wvx"
        };

        private static readonly Dictionary<string, string> m_MimeTypes = new Dictionary<string, string>()
        {
            {".*","application/octet-stream"},
            {".323","text/h323"},
            {".acx","application/internet-property-stream"},
            {".ai","application/postscript"},
            {".aif","audio/x-aiff"},
            {".aifc","audio/x-aiff"},
            {".aiff","audio/x-aiff"},
            {".asf","video/x-ms-asf"},
            {".asr","video/x-ms-asf"},
            {".asx","video/x-ms-asf"},
            {".au","audio/basic"},
            {".avi","video/x-msvideo"},
            {".axs","application/olescript"},
            {".bas","text/plain"},
            {".bcpio","application/x-bcpio"},
            {".bin","application/octet-stream"},
            {".bmp","image/bmp"},
            {".c","text/plain"},
            {".cat","application/vnd.ms-pkiseccat"},
            {".cdf","application/x-cdf"},
            {".cer","application/x-x509-ca-cert"},
            {".class","application/octet-stream"},
            {".clp","application/x-msclip"},
            {".cmx","image/x-cmx"},
            {".cod","image/cis-cod"},
            {".cpio","application/x-cpio"},
            {".crd","application/x-mscardfile"},
            {".crl","application/pkix-crl"},
            {".crt","application/x-x509-ca-cert"},
            {".csh","application/x-csh"},
            {".css","text/css"},
            {".dcr","application/x-director"},
            {".der","application/x-x509-ca-cert"},
            {".dir","application/x-director"},
            {".dll","application/x-msdownload"},
            {".dms","application/octet-stream"},
            {".doc","application/msword"},
            {".dot","application/msword"},
            {".dvi","application/x-dvi"},
            {".dxr","application/x-director"},
            {".eps","application/postscript"},
            {".etx","text/x-setext"},
            {".evy","application/envoy"},
            {".exe","application/octet-stream"},
            {".fif","application/fractals"},
            {".flr","x-world/x-vrml"},
            {".gif","image/gif"},
            {".gtar","application/x-gtar"},
            {".gz","application/x-gzip"},
            {".h","text/plain"},
            {".hdf","application/x-hdf"},
            {".hlp","application/winhlp"},
            {".hqx","application/mac-binhex40"},
            {".hta","application/hta"},
            {".htc","text/x-component"},
            {".htm","text/html"},
            {".html","text/html"},
            {".htt","text/webviewhtml"},
            {".ico","image/x-icon"},
            {".ief","image/ief"},
            {".iii","application/x-iphone"},
            {".ins","application/x-internet-signup"},
            {".isp","application/x-internet-signup"},
            {".jfif","image/pipeg"},
            {".jpe","image/jpeg"},
            {".jpeg","image/jpeg"},
            {".jpg","image/jpeg"},
            {".js","application/x-javascript"},
            {".latex","application/x-latex"},
            {".lha","application/octet-stream"},
            {".lsf","video/x-la-asf"},
            {".lsx","video/x-la-asf"},
            {".lzh","application/octet-stream"},
            {".m13","application/x-msmediaview"},
            {".m14","application/x-msmediaview"},
            {".m3u","audio/x-mpegurl"},
            {".man","application/x-troff-man"},
            {".mdb","application/x-msaccess"},
            {".me","application/x-troff-me"},
            {".mht","message/rfc822"},
            {".mhtml","message/rfc822"},
            {".mid","audio/mid"},
            {".mny","application/x-msmoney"},
            {".mov","video/quicktime"},
            {".movie","video/x-sgi-movie"},
            {".mp2","video/mpeg"},
            {".mp3","audio/mpeg"},
            {".mpa","video/mpeg"},
            {".mpe","video/mpeg"},
            {".mpeg","video/mpeg"},
            {".mpg","video/mpeg"},
            {".mpp","application/vnd.ms-project"},
            {".mpv2","video/mpeg"},
            {".ms","application/x-troff-ms"},
            {".msg","application/vnd.ms-outlook"},
            {".mvb","application/x-msmediaview"},
            {".nc","application/x-netcdf"},
            {".nws","message/rfc822"},
            {".oda","application/oda"},
            {".p10","application/pkcs10"},
            {".p12","application/x-pkcs12"},
            {".p7b","application/x-pkcs7-certificates"},
            {".p7c","application/x-pkcs7-mime"},
            {".p7m","application/x-pkcs7-mime"},
            {".p7r","application/x-pkcs7-certreqresp"},
            {".p7s","application/x-pkcs7-signature"},
            {".pbm","image/x-portable-bitmap"},
            {".pdf","application/pdf"},
            {".pfx","application/x-pkcs12"},
            {".pgm","image/x-portable-graymap"},
            {".pko","application/ynd.ms-pkipko"},
            {".pma","application/x-perfmon"},
            {".pmc","application/x-perfmon"},
            {".pml","application/x-perfmon"},
            {".pmr","application/x-perfmon"},
            {".pmw","application/x-perfmon"},
            {".pnm","image/x-portable-anymap"},
            {".pot","application/vnd.ms-powerpoint"},
            {".ppm","image/x-portable-pixmap"},
            {".pps","application/vnd.ms-powerpoint"},
            {".ppt","application/vnd.ms-powerpoint"},
            {".prf","application/pics-rules"},
            {".ps","application/postscript"},
            {".pub","application/x-mspublisher"},
            {".qt","video/quicktime"},
            {".ra","audio/x-pn-realaudio"},
            {".ram","audio/x-pn-realaudio"},
            {".ras","image/x-cmu-raster"},
            {".rgb","image/x-rgb"},
            {".rmi","audio/mid"},
            {".roff","application/x-troff"},
            {".rtf","application/rtf"},
            {".rtx","text/richtext"},
            {".scd","application/x-msschedule"},
            {".sct","text/scriptlet"},
            {".setpay","application/set-payment-initiation"},
            {".setreg","application/set-registration-initiation"},
            {".sh","application/x-sh"},
            {".shar","application/x-shar"},
            {".sit","application/x-stuffit"},
            {".snd","audio/basic"},
            {".spc","application/x-pkcs7-certificates"},
            {".spl","application/futuresplash"},
            {".src","application/x-wais-source"},
            {".sst","application/vnd.ms-pkicertstore"},
            {".stl","application/vnd.ms-pkistl"},
            {".stm","text/html"},
            {".sv4cpio","application/x-sv4cpio"},
            {".sv4crc","application/x-sv4crc"},
            {".svg","image/svg+xml"},
            {".swf","application/x-shockwave-flash"},
            {".t","application/x-troff"},
            {".tar","application/x-tar"},
            {".tcl","application/x-tcl"},
            {".tex","application/x-tex"},
            {".texi","application/x-texinfo"},
            {".texinfo","application/x-texinfo"},
            {".tgz","application/x-compressed"},
            {".tif","image/tiff"},
            {".tiff","image/tiff"},
            {".tr","application/x-troff"},
            {".trm","application/x-msterminal"},
            {".tsv","text/tab-separated-values"},
            {".txt","text/plain"},
            {".uls","text/iuls"},
            {".ustar","application/x-ustar"},
            {".vcf","text/x-vcard"},
            {".vrml","x-world/x-vrml"},
            {".wav","audio/x-wav"},
            {".wcm","application/vnd.ms-works"},
            {".wdb","application/vnd.ms-works"},
            {".wks","application/vnd.ms-works"},
            {".wmf","application/x-msmetafile"},
            {".wps","application/vnd.ms-works"},
            {".wri","application/x-mswrite"},
            {".wrl","x-world/x-vrml"},
            {".wrz","x-world/x-vrml"},
            {".xaf","x-world/x-vrml"},
            {".xbm","image/x-xbitmap"},
            {".xla","application/vnd.ms-excel"},
            {".xlc","application/vnd.ms-excel"},
            {".xlm","application/vnd.ms-excel"},
            {".xls","application/vnd.ms-excel"},
            {".xlt","application/vnd.ms-excel"},
            {".xlw","application/vnd.ms-excel"},
            {".xof","x-world/x-vrml"},
            {".xpm","image/x-xpixmap"},
            {".xwd","image/x-xwindowdump"},
            {".z","application/x-compress"},
            {".zip","application/zip"},
        };
        #endregion

        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, 55, Help.HelpPages);

            LogEntry("FileContentFormatter - Init success", LogEntryType.General);
        }

        public override string Format(string raw, ContextInformation context, FormattingPhase phase)
        {
            try
            {
                if(    context.Context == FormattingContext.PageContent
                    && context.ForIndexing == false
                    && context.ForWysiwyg == false)
                {
                    switch (phase)
                    {
                        case FormattingPhase.Phase1:
                            var matches = TagRegex.Matches(raw);
                            if (matches != null && matches.Count > 0)
                            {
                                //Get current file providers
                                var providers = m_Host.GetFilesStorageProviders(true);

                                if (providers.Length <= 0)
                                {
                                    throw new Exception("No active FileStorageProviders found!");
                                }

                                //Foreach query
                                foreach (Match match in matches)
                                {
                                    IFilesStorageProviderV30 stoProvider = providers[providers.Length - 1]; 
                                    string path = string.Empty;
                                    string filePattern = string.Empty;
                                    string storageProvider = string.Empty;
                                    string style = string.Empty;
                                    string rawStyle = string.Empty;

                                    var args = new ArgumentParser().Parse(match.Groups["arguments"].Value);

                                    //Get args
                                    string tmpPattern = (args.ContainsKey("file") == true ? args["file"] : "/*.*");
                                    path = tmpPattern.Substring(0, tmpPattern.LastIndexOf('/') + 1);
                                    filePattern = tmpPattern.Substring(tmpPattern.LastIndexOf('/') + 1);

                                    storageProvider = (args.ContainsKey("prov") == true ? args["prov"].ToLower() : "");
                                    bool displayRawData = false;
                                    bool.TryParse((args.ContainsKey("raw") == true ? args["raw"] : "false"), out displayRawData);

                                    //Customization/styling
                                    string height = (args.ContainsKey("height") == true ? args["height"].ToLower() : "");
                                    string width = (args.ContainsKey("width") == true ? args["width"].ToLower() : "");
                                    if (string.IsNullOrEmpty(height) == false) style = string.Format("height=\"{0}\"", height);
                                    if (string.IsNullOrEmpty(width) == false) style = (string.IsNullOrEmpty(style) == true ? string.Format("width=\"{0}\"", width) : string.Format("{0} width=\"{1}\"", style, width));

                                    string rows = (args.ContainsKey("rows") == true ? args["rows"].ToLower() : "");
                                    string cols = (args.ContainsKey("cols") == true ? args["cols"].ToLower() : "");
                                    if (string.IsNullOrEmpty(rows) == false) rawStyle = string.Format("rows=\"{0}\"", rows);
                                    if (string.IsNullOrEmpty(cols) == false) rawStyle = (string.IsNullOrEmpty(rawStyle) == true ? string.Format("cols=\"{0}\"", cols) : string.Format("{0} cols=\"{1}\"", rawStyle, cols));

                                    string tmpRawStyle = "";
                                    if (string.IsNullOrEmpty(height) == false) tmpRawStyle = string.Format("height: {0};", height);
                                    if (string.IsNullOrEmpty(width) == false) tmpRawStyle = (string.IsNullOrEmpty(tmpRawStyle) == true ? string.Format("width: {0};", width) : string.Format("{0} width: {1};", tmpRawStyle, width));

                                    if(string.IsNullOrEmpty(tmpRawStyle) == false)
                                    {
                                        rawStyle = string.Format("style=\"{0}\" {1}", tmpRawStyle, rawStyle);
                                    }

                                    //Find matching provider, use default if none
                                    foreach (var prov in providers)
                                    {
                                        if (prov.Information.Name.ToLower() == storageProvider)
                                        {
                                            stoProvider = prov;
                                            break;
                                        }
                                        //Else if default provider is allready set
                                    }

                                    string list = string.Format("(No files found matching \"{0}\".)", filePattern);

                                    //Get info from database
                                    var fileList = FindFiles(stoProvider, path, filePattern, ref list);

                                    //Generate content
                                    var content = GenerateContent(stoProvider, fileList, style, rawStyle, displayRawData);

                                    //Override only if there something there
                                    if (string.IsNullOrEmpty(content) == false)
                                    {
                                        list = content;
                                    }

                                    //Add a final newline
                                    list = string.Format("{0}", list);

                                    //Insert list
                                    //Recall position as string may allready have been modified by other table match entry
                                    int pos = TagRegex.Match(raw).Index;
                                    int length = TagRegex.Match(raw).Length;
                                    raw = raw.Remove(pos, length);
                                    raw = raw.Insert(pos, list);
                                }
                            }
                            break;
                        case FormattingPhase.Phase2:
                            break;
                        case FormattingPhase.Phase3:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                LogEntry(string.Format("FileContentFormatter error: {0} {1}", e.Message, e.StackTrace), LogEntryType.Error);
            }

            return raw;
        }

        private List<string> FindFiles(IFilesStorageProviderV30 _provider, string _path, string _filePattern,ref string _list)
        {
            var retval = new List<string>();

            if (_provider != null)
            {
                try
                {
                    var files = _provider.ListFiles(_path);

                    //Create usable regex from wildcard
                    var regex = Regex.Escape(_filePattern).Replace(@"\*", ".*").Replace(@"\?", ".");

                    foreach (var file in files)
                    {
                        //Add only if pattern match
                        if (Regex.IsMatch(file, regex) == true)
                        {
                            retval.Add(file);
                        }
                    }
                }
                catch (Exception e)
                {
                    var tmp = e.Message;
                    _list = string.Format("(No directory found matching \"{0}\".)", _path);
                }
            }

            return retval;
        }

        private string GenerateContent(IFilesStorageProviderV30 _provider, List<string> _files, string _style, string _rawStyle, bool _displayRawData)
        {
            string retval = "";

            if (_files.Count > 0)
            {
                foreach (var file in _files)
                {
                    string fileContent = string.Empty;

                    fileContent = GenerateContentBasedOnType(_provider, file, _style, _rawStyle, _displayRawData);

                    //Only add if theres something there
                    if (string.IsNullOrEmpty(fileContent) == false)
                    {
                        if (string.IsNullOrEmpty(retval) == false)
                        {
                            retval = string.Format("{0} <br></br> {1}", retval, fileContent);
                        }
                        else
                        {
                            retval = fileContent;
                        }
                    }
                }
            }

            return retval;
        }

        private string GenerateContentBasedOnType(IFilesStorageProviderV30 _provider, string _file, string _style, string _rawStyle, bool _displayRawData)
        {
            string retval = string.Empty;
            var extension = Path.GetExtension(_file).ToLower();

            var encodedFile = HttpUtility.UrlEncode(_file);

            if (m_MediaPlayerTypes.Contains(extension) == true)
            {
                retval = string.Format(@"<object {0} classid=""CLSID:6BF52A52-394A-11d3-B153-00C04F79FAA6"" VIEWASTEXT>\n" +
                                        @"<param name=""autoStart"" value=""False""></param>\n" +
                                        @"<param name=""URL"" value=""GetFile.aspx?File={1}&NoHit=1""></param>\n" +
                                        @"<param name=""enabled"" value=""True""></param>\n" +
                                        @"<param name=""balance"" value=""0""></param>\n" +
                                        @"<param name=""currentPosition"" value=""0""></param>\n" +
                                        @"<param name=""enableContextMenu"" value=""False""></param>\n" +
                                        @"<param name=""fullScreen"" value=""False""></param>\n" +
                                        @"<param name=""mute"" value=""False""></param>\n" +
                                        @"<param name=""playCount"" value=""1""></param>\n" +
                                        @"<param name=""rate"" value=""1""></param>\n" +
                                        @"<param name=""stretchToFit"" value=""False""></param>\n" +
                                        @"<param name=""uiMode"" value=""full""></param>\n" +
                                        @"</object>", 
                                        (string.IsNullOrEmpty(_style) == true ? "" : _style),
                                        encodedFile);
            }
            else if (_displayRawData == false) //Default
            {
                string type = "text/plain";

                if (m_MimeTypes.ContainsKey(extension) == true)
                {
                    type = m_MimeTypes[extension];
                }

                //Styled?
                if (string.IsNullOrEmpty(_style) == false)
                {
                    retval = string.Format(@"<object data=""GetFile.aspx?File={0}"" type=""{1}"" {2} ><embed src=""GetFile.aspx?File={0}"" type=""{1}"" {2}></embed></object>", encodedFile, type, _style);
                }
                else
                {
                    retval = string.Format(@"<object data=""GetFile.aspx?File={0}"" type=""{1}""><embed src=""GetFile.aspx?File={0}"" type=""{1}""></embed></object>", encodedFile, type);
                }
            }
            else //Raw display
            {

                var ms = new MemoryStream();

                try
                {
                    if (_provider.RetrieveFile(_file, ms, false) == true)
                    {
                        var reader = new StreamReader(ms);
                        ms.Position = 0;

                        retval = string.Format("{0}", reader.ReadToEnd());
                        reader.Close();
                    }

                    if (string.IsNullOrEmpty(_rawStyle) == false)
                    {
                        retval = string.Format("<textarea {0}><nowiki><nobr>{1}</nobr></nowiki></textarea>", _rawStyle, retval);
                    }
                }
                catch (Exception e)
                {
                    LogEntry(string.Format("FileContentFormatter error parsing file: {0} - {1} {2}", _file, e.Message, e.StackTrace), LogEntryType.Error);
                }
                finally
                {
                    ms.Close();
                }
            }

            return retval;
        }
    }
}
