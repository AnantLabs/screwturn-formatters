using System;
using System.Collections.Generic;
using System.Text;
using Keeper.Garrett.ScrewTurn.Core;
using System.Text.RegularExpressions;
using ScrewTurn.Wiki.PluginFramework;
using ScrewTurn.Wiki;
using System.Linq;
using System.IO;
using System.Globalization;
using Keeper.Garrett.ScrewTurn.Utility;

namespace Keeper.Garrett.ScrewTurn.FileContentFormatter
{
    public class MessageFormatter : FormatterBase
    {
        public override bool PerformPhase1 { get { return true; } }
        public override bool PerformPhase2 { get { return false; } }
        public override bool PerformPhase3 { get { return false; } }

        private static readonly Regex TagRegex = new Regex(@"\<msg(?<attributes>((.|\n|\r)+?))\>(?<msg>((.|\n|\r)+?))\</msg\>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        public override ComponentInformation  Information
        {
            get
            {
                return new ComponentInformation(this.GetType().Name, "Christian Hollerup Mikkelsen", string.Format("{0}", this.GetType().Assembly.GetName().Version), "http://keeper.endoftheinternet.org/", "http://keeper.endoftheinternet.org/GetFile.aspx?File=%2fScrewTurn%20-%20Formatters%20-%20Releases%2fScrewTurn%203.0.2.509%2fUpdates%2fMessageFormatter.txt&AsStreamAttachment=1&Provider=ScrewTurn.Wiki.FilesStorageProvider&NoHit=1");
            }
        }

        public override void Init(IHostV30 _host, string _config)
        {
            base.Init(_host, _config, 55, Help.HelpPages);

            StoreFiles(_host);

            LogEntry("MessageFormatter - Init success", LogEntryType.General);
        }

        private void StoreFiles(IHostV30 _host)
        {
            try
            {
                var persister = new FilePersister("MessageFormatter");
                persister.AddDir("Images-24x24");
                persister.AddDir("Images-32x32");

                //Style
                persister.AddFile("/", "MessageStyle.css", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.MessageStyle.css");

                //24x24
                persister.AddFile("Images-24x24", "add_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.add_blue.png");
                persister.AddFile("Images-24x24", "add_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.add_green.png");
                persister.AddFile("Images-24x24", "add_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.add_grey.png");
                persister.AddFile("Images-24x24", "add_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.add_lightblue.png");
                persister.AddFile("Images-24x24", "add_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.add_red.png");
                persister.AddFile("Images-24x24", "add_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.add_yellow.png");
                persister.AddFile("Images-24x24", "delete_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.delete_blue.png");
                persister.AddFile("Images-24x24", "delete_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.delete_green.png");
                persister.AddFile("Images-24x24", "delete_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.delete_grey.png");
                persister.AddFile("Images-24x24", "delete_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.delete_lightblue.png");
                persister.AddFile("Images-24x24", "delete_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.delete_red.png");
                persister.AddFile("Images-24x24", "delete_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.delete_yellow.png");
                persister.AddFile("Images-24x24", "empty_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.empty_blue.png");
                persister.AddFile("Images-24x24", "empty_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.empty_green.png");
                persister.AddFile("Images-24x24", "empty_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.empty_grey.png");
                persister.AddFile("Images-24x24", "empty_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.empty_lightblue.png");
                persister.AddFile("Images-24x24", "empty_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.empty_red.png");
                persister.AddFile("Images-24x24", "empty_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.empty_yellow.png");
                persister.AddFile("Images-24x24", "error_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.error_blue.png");
                persister.AddFile("Images-24x24", "error_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.error_green.png");
                persister.AddFile("Images-24x24", "error_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.error_grey.png");
                persister.AddFile("Images-24x24", "error_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.error_lightblue.png");
                persister.AddFile("Images-24x24", "error_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.error_red.png");
                persister.AddFile("Images-24x24", "error_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.error_yellow.png");
                persister.AddFile("Images-24x24", "exclamation_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.exclamation_blue.png");
                persister.AddFile("Images-24x24", "exclamation_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.exclamation_green.png");
                persister.AddFile("Images-24x24", "exclamation_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.exclamation_grey.png");
                persister.AddFile("Images-24x24", "exclamation_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.exclamation_lightblue.png");
                persister.AddFile("Images-24x24", "exclamation_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.exclamation_red.png");
                persister.AddFile("Images-24x24", "exclamation_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.exclamation_yellow.png");
                persister.AddFile("Images-24x24", "information_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.information_blue.png");
                persister.AddFile("Images-24x24", "information_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.information_green.png");
                persister.AddFile("Images-24x24", "information_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.information_grey.png");
                persister.AddFile("Images-24x24", "information_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.information_lightblue.png");
                persister.AddFile("Images-24x24", "information_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.information_red.png");
                persister.AddFile("Images-24x24", "information_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.information_yellow.png");
                persister.AddFile("Images-24x24", "left_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.left_blue.png");
                persister.AddFile("Images-24x24", "left_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.left_green.png");
                persister.AddFile("Images-24x24", "left_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.left_grey.png");
                persister.AddFile("Images-24x24", "left_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.left_lightblue.png");
                persister.AddFile("Images-24x24", "left_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.left_red.png");
                persister.AddFile("Images-24x24", "left_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.left_yellow.png");
                persister.AddFile("Images-24x24", "question_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.question_blue.png");
                persister.AddFile("Images-24x24", "question_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.question_green.png");
                persister.AddFile("Images-24x24", "question_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.question_grey.png");
                persister.AddFile("Images-24x24", "question_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.question_lightblue.png");
                persister.AddFile("Images-24x24", "question_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.question_red.png");
                persister.AddFile("Images-24x24", "question_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.question_yellow.png");
                persister.AddFile("Images-24x24", "right_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.right_blue.png");
                persister.AddFile("Images-24x24", "right_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.right_green.png");
                persister.AddFile("Images-24x24", "right_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.right_grey.png");
                persister.AddFile("Images-24x24", "right_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.right_lightblue.png");
                persister.AddFile("Images-24x24", "right_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.right_red.png");
                persister.AddFile("Images-24x24", "right_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.right_yellow.png");
                persister.AddFile("Images-24x24", "warning.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Medium.warning.png");

                //32x32
                persister.AddFile("Images-32x32", "add_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.add_blue.png");
                persister.AddFile("Images-32x32", "add_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.add_green.png");
                persister.AddFile("Images-32x32", "add_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.add_grey.png");
                persister.AddFile("Images-32x32", "add_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.add_lightblue.png");
                persister.AddFile("Images-32x32", "add_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.add_red.png");
                persister.AddFile("Images-32x32", "add_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.add_yellow.png");
                persister.AddFile("Images-32x32", "delete_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.delete_blue.png");
                persister.AddFile("Images-32x32", "delete_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.delete_green.png");
                persister.AddFile("Images-32x32", "delete_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.delete_grey.png");
                persister.AddFile("Images-32x32", "delete_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.delete_lightblue.png");
                persister.AddFile("Images-32x32", "delete_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.delete_red.png");
                persister.AddFile("Images-32x32", "delete_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.delete_yellow.png");
                persister.AddFile("Images-32x32", "empty_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.empty_blue.png");
                persister.AddFile("Images-32x32", "empty_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.empty_green.png");
                persister.AddFile("Images-32x32", "empty_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.empty_grey.png");
                persister.AddFile("Images-32x32", "empty_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.empty_lightblue.png");
                persister.AddFile("Images-32x32", "empty_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.empty_red.png");
                persister.AddFile("Images-32x32", "empty_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.empty_yellow.png");
                persister.AddFile("Images-32x32", "error_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.error_blue.png");
                persister.AddFile("Images-32x32", "error_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.error_green.png");
                persister.AddFile("Images-32x32", "error_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.error_grey.png");
                persister.AddFile("Images-32x32", "error_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.error_lightblue.png");
                persister.AddFile("Images-32x32", "error_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.error_red.png");
                persister.AddFile("Images-32x32", "error_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.error_yellow.png");
                persister.AddFile("Images-32x32", "exclamation_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.exclamation_blue.png");
                persister.AddFile("Images-32x32", "exclamation_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.exclamation_green.png");
                persister.AddFile("Images-32x32", "exclamation_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.exclamation_grey.png");
                persister.AddFile("Images-32x32", "exclamation_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.exclamation_lightblue.png");
                persister.AddFile("Images-32x32", "exclamation_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.exclamation_red.png");
                persister.AddFile("Images-32x32", "exclamation_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.exclamation_yellow.png");
                persister.AddFile("Images-32x32", "information_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.information_blue.png");
                persister.AddFile("Images-32x32", "information_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.information_green.png");
                persister.AddFile("Images-32x32", "information_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.information_grey.png");
                persister.AddFile("Images-32x32", "information_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.information_lightblue.png");
                persister.AddFile("Images-32x32", "information_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.information_red.png");
                persister.AddFile("Images-32x32", "information_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.information_yellow.png");
                persister.AddFile("Images-32x32", "left_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.left_blue.png");
                persister.AddFile("Images-32x32", "left_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.left_green.png");
                persister.AddFile("Images-32x32", "left_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.left_grey.png");
                persister.AddFile("Images-32x32", "left_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.left_lightblue.png");
                persister.AddFile("Images-32x32", "left_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.left_red.png");
                persister.AddFile("Images-32x32", "left_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.left_yellow.png");
                persister.AddFile("Images-32x32", "question_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.question_blue.png");
                persister.AddFile("Images-32x32", "question_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.question_green.png");
                persister.AddFile("Images-32x32", "question_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.question_grey.png");
                persister.AddFile("Images-32x32", "question_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.question_lightblue.png");
                persister.AddFile("Images-32x32", "question_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.question_red.png");
                persister.AddFile("Images-32x32", "question_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.question_yellow.png");
                persister.AddFile("Images-32x32", "right_blue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.right_blue.png");
                persister.AddFile("Images-32x32", "right_green.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.right_green.png");
                persister.AddFile("Images-32x32", "right_grey.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.right_grey.png");
                persister.AddFile("Images-32x32", "right_lightblue.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.right_lightblue.png");
                persister.AddFile("Images-32x32", "right_red.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.right_red.png");
                persister.AddFile("Images-32x32", "right_yellow.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.right_yellow.png");
                persister.AddFile("Images-32x32", "warning.png", "Keeper.Garrett.ScrewTurn.MessageFormatter.Resources.Images.Large.warning.png");

                persister.StoreFiles(_host);
            }
            catch (Exception e)
            {
                LogEntry(string.Format("MessageFormatter - StoreFiles - Error: {0}", e.Message), LogEntryType.Error);
            }
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
                                //Foreach query
                                foreach (Match match in matches)
                                {
                                    var args = new ArgumentParser().Parse(match.Groups["attributes"].Value);

                                    //Get args
                                    var style = (args.ContainsKey("type") == true ? args["type"] : "info");
                                    var head = (args.ContainsKey("head") == true ? args["head"] : "");
                                    string msg = match.Groups["msg"].Value;

                                    //Generate content
                                    var content = string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"GetFile.aspx?File=/Keeper.Garrett.Formatters/MessageFormatter/MessageStyle.css\"></link>");
                                    content = string.Format("{0}\n<table class=\"{1}\"><tbody>", content, style.ToLower());

                                    if (string.IsNullOrEmpty(head) == false)
                                    {
                                        content = string.Format("{0}\n<tr><td class=\"image-col\"></td><td class=\"head-col\">{1}</td</tr>", content, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(head));
                                    }
                                    else
                                    {
                                        content = string.Format("{0}\n<tr><td class=\"image-col\"></td><td class=\"head-col\">{1}</td</tr>", content, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(style));
                                    }

                                    if (string.IsNullOrEmpty(msg) == false)
                                    {
                                        content = string.Format("{0}\n<tr><td></td><td class=\"content\">{1}</td></tr>", content, msg);
                                    }

                                    content = string.Format("{0}</tbody></table>", content);

                                    //Insert list
                                    //Recall position as string may allready have been modified by other table match entry
                                    int pos = TagRegex.Match(raw).Index;
                                    int length = TagRegex.Match(raw).Length;
                                    raw = raw.Remove(pos, length);
                                    raw = raw.Insert(pos, content);
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
                LogEntry(string.Format("MessageFormatter error: {0} {1}", e.Message, e.StackTrace), LogEntryType.Error);
            }

            return raw;
        }
    }
}
