![http://allscion.com/images/Discontinued.png](http://allscion.com/images/Discontinued.png)
# Due to the fact that http://www.screwturn.eu/is being discontinued this project is also discontinued. However the code will of course still be avaliable to anyone how might be interested here at google code. #

<table width='100%' border='0'>
<tr>
<td>
<h1>Project Summary</h1>
<br>
<br>
<hr><br>
<br>
<br>
<br>
</hr><br>
<br>
<br>
</td>
<td width='50px'></td>
<td width='240px'>
<h1>Status</h1>
<br>
<br>
<hr><br>
<br>
<br>
<br>
</hr><br>
<br>
<br>
</td>
</tr>
</table>

<table width='100%' border='0'>
<tr>
<td>
The goal of this project is to add a collection of formatter plugins for the wonderfull <a href='http://www.screwturn.eu'>http://www.screwturn.eu</a>. All the source have been moved from my personal server (Mercurial, SVN + my screwturn blog) to google code, to make the source accessible to all , to get better issue tracking and to get some collaboration going :).<br>
Check out the different sections in short:<br>
<br>
<ul><li><a href='#Formatters.md'>Formatters</a>
</li><li><a href="#Got_issue's,_bugs,_requests?.md">Got issue's, bugs, requests?</a>
</li><li><a href='#Examples.md'>Examples</a>
</li><li><a href='#Developer_Information.md'>Developer Information</a>
</li><li><a href='http://screwturn-formatters.googlecode.com/hg/ChangeLog.txt'>To see the ChangeLog go here</a></li></ul>

<b>Note:</b>
<blockquote><i>This project has it's own buildserver for testing, integration testing and automated release to the public. See the status display on the right</i> (I think that's <i>very</i> nice, but then again I'm a geek :-D)<br>
</td>
<td width='50px'></td>
<td width='260px'>
<table align='right' border='0'>
<tr><td>
<img src='http://wiki.screwturn-formatters.googlecode.com/hg/status.png' />
<wiki:gadget url="http://hosting.gmodules.com/ig/gadgets/file/110939238245911658652/GoogleCodeDownloadsList-ScrewTurn.xml" border="0" height="440" width="240" title="Downloads" up_extraCount="2524"/><br>
</td></tr>
</table>
</td>
</tr>
<tr>
<td height='100'>
<h1>Formatters</h1>
<hr />
Currently the following set of formatters are avaliable (see download section):</blockquote>

<ul><li><b>BlogFormatter</b> - Create a Blog inside your ScrewTurn Wiki.<br>
</li><li><b>CategoryListFormatter</b> - List all pages within a certain category.<br>
</li><li><b>EventLogFormatter</b> - List content of eventlogs.<br>
</li><li><b>FileContentFormatter</b> - Display file content dynamically in a wiki page<br>
</li><li><b>FileListFormatter</b> - List files in a directory in one of your file storage providers<br>
</li><li><b>QueryTableFormatter</b> - Query Oracle/MsSql/MySql/SqLite db's and get dynamic results in table format.<br>
</li><li><b>MessageFormatter</b> - Display a highlighted message in a wiki page.</li></ul>

<b>Note:</b> The formatters only have been tested and verified to work against specific versions of <a href='http://www.screwturn.eu'>STW</a>. If you use newer/older versions of <a href='http://www.screwturn.eu'>STW</a> there's no guarentee that the formatters will work.<br>
</td>
<td width='50px'></td>
<td> </td></tr>
<tr>
<td> </td>
<td width='50px'></td>
<td> </td>
</tr>
</table>
# Got issue's, bugs, requests? #

---

Head over to the [Issue tracking section](http://code.google.com/p/screwturn-formatters/issues/list) and start typing :).

<br />
# Examples #

---


<br />
# Developer Information #

---

Here's some of the project facts for developers:
  * Most important of all:
    * **`This project has it's own buildserver for testing, integration testing and automated release to this project page.`**
  * The project makes uses of a lot of TDD (but in it's own unique way :)).
  * The project uses the following tools, which it is recommended you have a basic knowledge off:
    * [RhinoMocks](http://www.ayende.com/projects/rhino-mocks.aspx)
    * [NUnit](http://www.nunit.org/)
    * [NCover (the free version)](http://www.ncover.com/)
    * VS2010
    * VS2010 Code Metrics
