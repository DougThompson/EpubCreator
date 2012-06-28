# EPUB Creator #

**EPUB Creator** is a C# application designed to create non-DRM EPUB books.  Think of it as a simplified [Calibre](http://calibre-ebook.com/) or [Sigil](http://code.google.com/p/sigil/) that focuses on simplicity.  I wrote this application because (at the time) there was not a good way of creating EPUBs from HTML files.  Tools like Calibre or Sigil have improved and many more features, but I also wanted to control the look and layout of the EPUBs more because I was reading on the iPod Touch using Stanza at the time.  Now with better readers like the Nooks and Kindles, etc..., it is less of an issue.

I have ported much of this to a Python application and added the ability to edit existing EPUBs, too.  I have not back-ported the EPUB editing functionality to this codebase, however.

## Development Requirements ##

- Visual Studio 2010, C#, .NET Framework 3.5
- HtmlTidy exe
- [Ionic Zip library](http://dotnetzip.codeplex.com/) v1.6.3.18 (old version, true, but provided for ease)

## How to Use ##
The application can be used to create an EPUB file from a single HTML or Text file.  The application will enter the correct mode based on the file extension opened.

### Creating an EPUB ###
The application will parse the document meta data based on the following filename format (each section delimited by ' - ' ([space][dash][space]):

> {Book Name} - {Author Last, Author First} - {Publisher(s)} - {Year} - {Comma separated list of subjects}.html

The application expects an HTML file with up to 3 levels of chapters (usually based on &lt;H1&gt;, &lt;H2&gt;, and &lt;H3&gt;).  You can have other standard HTML tags as long as they are supported by your EPUB reader.  If the file is a Text file, it will try to parse the Chapters based on keywords, such as "CHAPTER".  I strongly recommend using HTML, however.

The application will automatically add a cover image if it is named:

> {Author Lastname (no spaces)}_{Title (no spaces)}.jpg

Once happy with the EPUB, simply save it to disk!

### Preferences ###
There are some settings in the app.config such as default directories, library file locations, tags and attributes to strip, etc....

## Future Plans, Roadmap ##
None at this time.

## License ##
Released under the [MIT License](http://www.opensource.org/licenses/mit-license.php)

Copyright (c) 2012 Doug Thompson

Permission is hereby granted, free of charge, to any person obtaining a
copy of this software and associated documentation files (the
"Software"),to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

