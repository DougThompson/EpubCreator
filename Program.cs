using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using System.IO;
using System.Xml;
using System.Text;
using System.Configuration;
using Ionic.Utils.Zip;
using System.Security.Cryptography;

namespace EpubCreator
{
	static class Program
	{
		public static string[] Args;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Args = args;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());
		}

        /// <summary>
        /// Gets a simple SHA-1 hash of a string and breaks it into a 5-part string, GUID style
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
		public static string GetHash(string Input)
		{
            // Set the encoding, the bugger, and grab the Crypto object
			Encoding enc = System.Text.Encoding.ASCII;
			byte[] buffer = enc.GetBytes(Input);
			SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();

            // Get the hash and remove any current "-" so that it can be converted into a GUID style hash
			string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "").ToLowerInvariant();

			hash = String.Format("{0}-{1}-{2}-{3}-{4}", hash.Substring(0, 8), hash.Substring(8, 4), hash.Substring(12, 4), hash.Substring(16, 4), hash.Substring(20, 12));
			return hash;
		}

        /// <summary>
        /// Gets a simple SHA-1 hash of a string and only returns the first 8 characters
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
		public static string GetShortHash(string Input)
		{
            // Set the encoding, the bugger, and grab the Crypto object
            Encoding enc = System.Text.Encoding.ASCII;
            byte[] buffer = enc.GetBytes(Input);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();

            // Get the hash and remove any current "-" so that the first 8 characters can be returned
            string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "").ToLowerInvariant();

			return hash.Substring(0, 8);
		}

        /// <summary>
        /// Create a thumbnail version of a book cover
        /// TODO:
        /// Cannot remember where some of this code was found online, so try
        /// to find the reference...
        /// </summary>
        /// <param name="ImageFileName"></param>
		public static void SaveThumbnail(string ImageFileName)
		{
			// We will store the correct image codec in this object
			ImageCodecInfo iciJpegCodec = null;
			// This will specify the image quality to the encoder
			EncoderParameter epQuality = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
			// Get all image codecs that are available
			ImageCodecInfo[] iciCodecs = ImageCodecInfo.GetImageEncoders();
			// Store the quality parameter in the list of encoder parameters
			EncoderParameters epParameters = new EncoderParameters(1);
			epParameters.Param[0] = epQuality;

			// Loop through all the image codecs
			for (int i = 0; i < iciCodecs.Length; i++)
			{
				// Until the one that we are interested in is found, which is image/jpeg
				if (iciCodecs[i].MimeType == "image/jpeg")
				{
					iciJpegCodec = iciCodecs[i];
					break;
				}
			}

			// Create a new Image object from the current file
			Image newImage = Image.FromFile(ImageFileName);

            // Set the image size, interpolation, smoothing mode, and other settings
            // to get the best thumbnail possible.
			Image thumbnail = new Bitmap(57, 80);
			System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnail);
			graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphic.SmoothingMode = SmoothingMode.HighQuality;
			graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
			graphic.CompositingQuality = CompositingQuality.HighQuality;
			graphic.DrawImage(newImage, 0, 0, 57, 80);

			// Get the file information again, this time we want to find out the extension
			FileInfo fiPicture = new FileInfo(ImageFileName);
			// Save the new file at the selected path with the specified encoder parameters, and reuse the same file name
			thumbnail.Save(fiPicture.DirectoryName + "\\" + fiPicture.Name.Replace(".jpg", "_tn.jpg"), iciJpegCodec, epParameters);
		}

        /// <summary>
        /// This function will obfuscate file and folder names to help with
        /// hiding any ebooks put up on a public folder, such as Dropbox.
        /// This may not be necessary if using a robots.txt, but just to be sure
        /// this will make it a bit harder to search for... don't want to run afoul
        /// of any copyright issues by making something public when meant to be
        /// for personal use only
        /// </summary>
		public static void Obfuscate()
		{
            // Grab the base dirs on the local (non-public) machine
			string BaseDir = ConfigurationManager.AppSettings["BaseDir"];
			string BaseDestDir = ConfigurationManager.AppSettings["PublicDestintionDir"];

            // Setup the namespaces necessary for the ATOM file
			XmlDocument xmlDoc = new XmlDocument();
			XmlNamespaceManager xmlNs = new XmlNamespaceManager(xmlDoc.NameTable);
			xmlNs.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
			xmlNs.AddNamespace("opf", "http://www.idpf.org/2007/opf");

            // Load the non-obfuscated library file
			xmlDoc = new XmlDocument();
			xmlDoc.Load(ConfigurationManager.AppSettings["LibraryFile"]);

            // Add the appropriate Namespaces to the document
			xmlNs = new XmlNamespaceManager(xmlDoc.NameTable);
			xmlNs.AddNamespace("x", "http://www.w3.org/2005/Atom");
			xmlNs.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

            // Start building the EPUB details and getting the variables ready
			String Author = "", Title = "", EpubFileName = "", Thumbnail = "", CoverImage = "";
			String SourceEpubFileName = "", SourceThumbnail = "", SourceCoverImage = "";
			String DestAuthor, DestTitle = "", DestEpubFileName = "", DestThumbnail = "", DestCoverImage = "";

            // Now loop through each entry in the non-obfuscated library and process it
			foreach (XmlNode item in xmlDoc.SelectNodes("//x:entry", xmlNs))
			{
                // Grab the author and title
				Author = item["author"]["name"].InnerText;
				Title = item["title"].InnerText;
				foreach (XmlNode link in item.ChildNodes)
				{
                    // If there are children nodes (and there should be) pull the rest
                    // of the EPUB details from them, especially any image links and
                    // the EPUB file name
					if (link.Name == "link")
					{
                        // This would be the EPUB file itself
						if (link.Attributes["type"].Value == "application/epub+zip")
						{
							SourceEpubFileName = link.Attributes["href"].Value;
							EpubFileName = SourceEpubFileName.Substring(SourceEpubFileName.LastIndexOf("/") + 1);
						}
						else
						{
                            // Check for any images, mainly the Source Cover and Thumbnail
							if (link.Attributes["rel"].Value == "x-stanza-cover-image-thumbnail")
							{
								SourceThumbnail = link.Attributes["href"].Value;
								Thumbnail = SourceThumbnail.Substring(SourceThumbnail.LastIndexOf("/") + 1);
								if (SourceThumbnail.IndexOf(".jpg") >= 0)
									if (SourceThumbnail.IndexOf("_tn") < 0)
										link.Attributes["href"].Value = SourceThumbnail.Substring(0, SourceThumbnail.LastIndexOf(".")) + "_tn" + SourceThumbnail.Substring(SourceThumbnail.LastIndexOf("."));
							}
							else if (link.Attributes["rel"].Value == "x-stanza-cover-image")
							{
								SourceCoverImage = link.Attributes["href"].Value;
								CoverImage = SourceCoverImage.Substring(SourceCoverImage.LastIndexOf("/") + 1);
								if (SourceCoverImage.IndexOf(".jpg") >= 0)
									SaveThumbnail((BaseDir + SourceCoverImage).Replace('/', '\\'));
							}
						}
					}
				}

                // Hash the non-ofuscated details
				DestAuthor = GetShortHash(Author);
				DestTitle = GetShortHash(Title);
				DestEpubFileName = String.Format("{0}/{1}.epub", DestAuthor, DestTitle);
				DestCoverImage = String.Format("{0}/{1}", DestAuthor, GetShortHash(CoverImage) + Path.GetExtension(CoverImage));
				DestThumbnail = String.Format("{0}/{1}", DestAuthor, GetShortHash(Thumbnail) + Path.GetExtension(Thumbnail));

                // Now loop back through the children and update the info
                // with the newly hashed values
				foreach (XmlNode link in item.ChildNodes)
				{
					if (link.Name == "link")
					{
						if (link.Attributes["type"].Value == "application/epub+zip")
						{
							link.Attributes["href"].Value = DestEpubFileName;
						}
						else
						{
							if (link.Attributes["rel"].Value == "x-stanza-cover-image-thumbnail")
							{
								link.Attributes["href"].Value = DestThumbnail;
							}
							else if (link.Attributes["rel"].Value == "x-stanza-cover-image")
							{
								link.Attributes["href"].Value = DestCoverImage;
							}
						}
					}
				}

                // Prepare to copy the EPUB and any images to the public location
				SourceEpubFileName = (BaseDir + SourceEpubFileName.Replace('/', '\\'));
				DestEpubFileName = (BaseDestDir + DestEpubFileName.Replace('/', '\\'));
				SourceCoverImage = (BaseDir + SourceCoverImage.Replace('/', '\\'));
				DestCoverImage = (BaseDestDir + DestCoverImage.Replace('/', '\\'));
				SourceThumbnail = (BaseDir + SourceThumbnail.Replace('/', '\\'));
				DestThumbnail = (BaseDestDir + DestThumbnail.Replace('/', '\\'));

                // Check to see if the directory exists, if not then create it
				if (!Directory.Exists(BaseDestDir + DestAuthor))
					Directory.CreateDirectory(BaseDestDir + DestAuthor);

                // Start copying files and overwrite if they exist
				FileInfo fileSource = null;
				FileInfo fileDest = null;
				if (File.Exists(DestEpubFileName))
				{
					fileSource = new FileInfo(SourceEpubFileName);
					fileDest = new FileInfo(DestEpubFileName);

					if (fileSource.LastWriteTime != fileDest.LastWriteTime)
						File.Copy(SourceEpubFileName, DestEpubFileName, true);
				}
				else
					File.Copy(SourceEpubFileName, DestEpubFileName, true);

				if (File.Exists(DestCoverImage))
				{
					fileSource = new FileInfo(SourceCoverImage);
					fileDest = new FileInfo(DestCoverImage);

					if (fileSource.LastWriteTime != fileDest.LastWriteTime)
						File.Copy(SourceCoverImage, DestCoverImage, true);
				}
				else
					File.Copy(SourceCoverImage, DestCoverImage, true);

				if (File.Exists(DestThumbnail))
				{
					fileSource = new FileInfo(SourceThumbnail);
					fileDest = new FileInfo(DestThumbnail);

					if (fileSource.LastWriteTime != fileDest.LastWriteTime)
						File.Copy(SourceThumbnail, DestThumbnail, true);
				}
				else
					File.Copy(SourceThumbnail, DestThumbnail, true);
			}

            // Save the library file and alert the user
			xmlDoc.Save(Path.Combine(BaseDestDir, "library.xml"));
			xmlDoc = null;

			MessageBox.Show("Done.");
		}

        /// <summary>
        /// This will open a current EPUB file and read the contents, and pull out the images
        /// </summary>
        /// <param name="EpubFileName"></param>
        /// <param name="lstImages"></param>
		public static void ReadEpub(String EpubFileName, ListBox lstImages)
		{
			String LibraryFile = ConfigurationManager.AppSettings["LibraryFile"];
			lstImages.Items.Clear();
			using (ZipFile zip = ZipFile.Read(EpubFileName))
			{
				foreach (ZipEntry e in zip)
				{
                    if ((e.FileName.ToLower().EndsWith("jpg")) || (e.FileName.ToLower().EndsWith("jpeg")) || (e.FileName.ToLower().EndsWith("png")) || (e.FileName.ToLower().EndsWith("gif")))
					{
						lstImages.Items.Add(e.FileName);
					}
				}
				foreach (ZipEntry e in zip)
				{
                    if ((e.FileName.ToLower().EndsWith("jpg")) || (e.FileName.ToLower().EndsWith("jpeg")) || (e.FileName.ToLower().EndsWith("png")) || (e.FileName.ToLower().EndsWith("gif")))
					{
						zip.Extract(e.FileName, "", true);
					}
				}
			}
			if (lstImages.Items.Count > 0)
				lstImages.SelectedIndex = 0;
		}

        /// <summary>
        /// Add an EPUB to an ATOM library file
        /// </summary>
        /// <param name="EpubFileName"></param>
        /// <param name="lstImages"></param>
        /// <param name="SubFolder"></param>
        /// <param name="AddToPublicLibrary"></param>
		public static void AddToLibrary(String EpubFileName, ListBox lstImages, String SubFolder, Boolean AddToPublicLibrary)
		{
            // Open the library file and prepare for writing
			String LibraryFile = ConfigurationManager.AppSettings["LibraryFile"];
			String FileName = "";
			String ImageExtension = "";
			String CoverImage = "";
			Boolean CopyImage = false;

            // If it is for the public library, switch paths
            if (AddToPublicLibrary)
                LibraryFile = Path.Combine(ConfigurationManager.AppSettings["PublicDestintionDir"], "library.xml");

            // Get the images currently assigned to the EPUB
			if (lstImages.Items.Count == 0)
			{
				CoverImage = Path.GetFileNameWithoutExtension(EpubFileName) + ".jpg";
				CopyImage = false;
			}
			else
			{
				if (lstImages.SelectedItem == null)
				{
					CoverImage = Path.GetFileNameWithoutExtension(EpubFileName) + ".jpg";
					CopyImage = false;
				}
				else
				{
					CoverImage = lstImages.SelectedItem.ToString();
					CopyImage = true;
				}
			}
			ImageExtension = Path.GetExtension(CoverImage);

            // Get the OPF and image files from the EPUB
			using (ZipFile zip = ZipFile.Read(EpubFileName))
			{
				foreach (ZipEntry e in zip)
				{
					if (e.FileName.EndsWith("opf"))
					{
						FileName = e.FileName;
						zip.Extract(e.FileName, "", true);
						break;
					}
				}
				foreach (ZipEntry e in zip)
				{
					if ((e.FileName.ToLower().EndsWith("jpg")) || (e.FileName.ToLower().EndsWith("png")) || (e.FileName.ToLower().EndsWith("gif")))
					{
						zip.Extract(e.FileName, "", true);
					}
				}
			}

            // Open the OPF file and set the correct Namespaces to extract
            // the EPUB details
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(FileName);
			XmlNamespaceManager xmlNs = new XmlNamespaceManager(xmlDoc.NameTable);
			xmlNs.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
			xmlNs.AddNamespace("opf", "http://www.idpf.org/2007/opf");

			String Title = "";
			String Author = "";
			String Identifier = "";
			String OrigPublishDate = "";
			String DateCreated = "";
			String Subjects = "";
			String Subfolder = SubFolder == "" ? "" : SubFolder + "/";

			Title = xmlDoc.SelectSingleNode("//dc:title", xmlNs).InnerText;
			Author = xmlDoc.SelectSingleNode("//dc:creator", xmlNs).Attributes["opf:file-as"].Value;
			Identifier = xmlDoc.SelectSingleNode("//dc:identifier", xmlNs).InnerText.Replace("urn:uuid:", "");
			OrigPublishDate = xmlDoc.SelectSingleNode("//dc:date[@opf:event=\"original-publication\"]", xmlNs).InnerText;
			DateCreated = xmlDoc.SelectSingleNode("//dc:date[@opf:event=\"ops-publication\"]", xmlNs).InnerText;

			DateTime dateCreated;
			if (!DateTime.TryParse(DateCreated, out dateCreated))
				dateCreated = DateTime.Now.ToUniversalTime();

            // Join the subjects together
			foreach (XmlNode node in xmlDoc.SelectNodes("//dc:subject", xmlNs))
			{
				Subjects += node.InnerText + ", ";
			}
			xmlDoc = null;
			xmlNs = null;

			String NewImageName = Path.GetFileNameWithoutExtension(EpubFileName) + ImageExtension;

            // Create a dummy xml tag to add the entry which will be extracted later and
            // added to the library file
			StringBuilder sb = new StringBuilder();
			sb.Append("<dummy xmlns=\"http://www.w3.org/2005/Atom\">\r\n");
			sb.Append("\t<entry>\r\n");
			sb.Append(String.Format("\t\t<title>{0}</title>\r\n", Title));
			sb.Append("\t\t<content type=\"xhtml\">\r\n");
			sb.Append(String.Format("\t\t\t<div xmlns=\"http://www.w3.org/1999/xhtml\"> Published: {0}, Language: en, Subject: {1}</div>\r\n", OrigPublishDate, Subjects));
			sb.Append("\t\t</content>\r\n");
			sb.Append(String.Format("\t\t<id>urn:udid:{0}</id>\r\n", Identifier));
			sb.Append("\t\t<author>\r\n");
			sb.Append(String.Format("\t\t\t<name>{0}</name>\r\n", Author));
			sb.Append("\t\t</author>\r\n");
			sb.Append(String.Format("\t\t<updated>{0}</updated>\r\n", dateCreated.ToUniversalTime().ToString("u")));

            String DestAuthor = "", DestTitle = "", DestEpubFileName = "", DestThumbnail = "", DestCoverImage = "";

            // Choose the public (non-obfuscated) library or the private (obfuscated) library
            if (!AddToPublicLibrary)
            {
                DestAuthor = Author;
                DestTitle = Title;
                DestEpubFileName = String.Format("{0}/{1}.epub", DestAuthor, DestTitle);
                DestCoverImage = String.Format("{0}/{1}", DestAuthor, NewImageName + Path.GetExtension(NewImageName));
                DestThumbnail = String.Format("{0}/{1}", DestAuthor, NewImageName + Path.GetExtension(NewImageName));
            }
            else
            {
                DestAuthor = GetShortHash(Author);
                DestTitle = GetShortHash(Title);
                DestEpubFileName = String.Format("{0}/{1}.epub", DestAuthor, DestTitle);
                DestCoverImage = String.Format("{0}/{1}", DestAuthor, GetShortHash(NewImageName) + Path.GetExtension(NewImageName));
                DestThumbnail = String.Format("{0}/{1}", DestAuthor, GetShortHash(NewImageName) + Path.GetExtension(NewImageName));
            }

            sb.Append(String.Format("\t\t<link type=\"application/epub+zip\" href=\"{0}/{1}.epub\"/>\r\n", DestAuthor, DestTitle));
            sb.Append(String.Format("\t\t<link rel=\"x-stanza-cover-image-thumbnail\" type=\"image/{1}\" href=\"{0}\"/>\r\n", DestCoverImage, Path.GetExtension(NewImageName).Replace(".", "")));
            sb.Append(String.Format("\t\t<link rel=\"x-stanza-cover-image\" type=\"image/{1}\" href=\"{0}\"/>\r\n", DestCoverImage, Path.GetExtension(NewImageName).Replace(".", "")));
            sb.Append("\t</entry>\r\n");
			sb.Append("</dummy>\r\n");

            // Copy any images to the folder
            string BaseDir = Path.GetDirectoryName(EpubFileName);
            string BaseDestDir = Path.GetDirectoryName(LibraryFile);
            if (!AddToPublicLibrary)
            {
                if (CopyImage)
                {
                    File.Copy(Path.GetFullPath(CoverImage), Path.Combine(Path.GetDirectoryName(LibraryFile), DestCoverImage), true);
                }

                if (File.Exists(Path.Combine(Path.GetDirectoryName(LibraryFile), DestCoverImage)))
                    SaveThumbnail(Path.Combine(Path.GetDirectoryName(LibraryFile), DestCoverImage));
            }
            else
            {
                if (!Directory.Exists(Path.Combine(BaseDestDir, DestAuthor)))
                    Directory.CreateDirectory(Path.Combine(BaseDestDir, DestAuthor));

                File.Copy(Path.GetFullPath(CoverImage), Path.Combine(BaseDestDir, DestCoverImage), true);
                
                String SourceEpubFileName = EpubFileName;
                DestEpubFileName = Path.Combine(BaseDestDir, DestEpubFileName).Replace('/', '\\');
                DestCoverImage = Path.Combine(BaseDestDir, DestCoverImage).Replace('/', '\\');

                FileInfo fileSource = null;
                FileInfo fileDest = null;
                if (File.Exists(DestEpubFileName))
                {
                    fileSource = new FileInfo(SourceEpubFileName);
                    fileDest = new FileInfo(DestEpubFileName);

                    if (fileSource.LastWriteTime != fileDest.LastWriteTime)
                        File.Copy(SourceEpubFileName, DestEpubFileName, true);
                }
                else
                    File.Copy(SourceEpubFileName, DestEpubFileName, true);

                if (File.Exists(DestCoverImage))
                {
                    fileSource = new FileInfo(Path.GetFullPath(CoverImage));
                    fileDest = new FileInfo(DestCoverImage);

                    if (fileSource.LastWriteTime != fileDest.LastWriteTime)
                        File.Copy(Path.GetFullPath(CoverImage), DestCoverImage, true);
                }
                else
                    File.Copy(Path.GetFullPath(CoverImage), DestCoverImage, true);
            }

            // Now we will loop through each item in the Library and see if we
            // are adding a new entry or updating an existing one.  We should
            // find an existing one based on the hashing method so we should not
            // be adding duplicates.
			Boolean ItemExists = false;
			xmlDoc = new XmlDocument();
			xmlDoc.Load(LibraryFile);

			xmlNs = new XmlNamespaceManager(xmlDoc.NameTable);
			xmlNs.AddNamespace("x", "http://www.w3.org/2005/Atom");
			xmlNs.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

			xmlDoc.SelectSingleNode("//x:updated", xmlNs).InnerText = DateTime.Now.ToUniversalTime().ToString("u");

			XmlNode entry = null;
			entry = xmlDoc.SelectSingleNode("//x:entry[x:id=\"urn:" + Identifier + "\"]", xmlNs);
			if (xmlDoc.SelectSingleNode("//x:entry[x:id=\"urn:" + Identifier + "\"]", xmlNs) != null)
				entry = xmlDoc.SelectSingleNode("//x:entry[x:id='urn:" + Identifier + "']", xmlNs);

			if (entry != null)
			{
				ItemExists = true;
			}

            // Grab the fragment created above
			XmlDocumentFragment xmlFragment = xmlDoc.CreateDocumentFragment();
			xmlFragment.InnerXml = sb.ToString();

            // It exists, so delete the current entry
			bool added = false;
			if (ItemExists)
				xmlDoc.DocumentElement.RemoveChild(entry);

			String itemBookKey = "";
			String newBookKey = "";
			int compare;

            // Now, go through each entry again and determine where it should be inserted
			foreach (XmlNode item in xmlDoc.SelectNodes("//x:entry", xmlNs))
			{
				itemBookKey = item["author"]["name"].InnerText + "-" + item["title"].InnerText;
				newBookKey = Author + "-" + Title;

				compare = newBookKey.CompareTo(itemBookKey);
				if (compare < 0)
				{
					xmlDoc.DocumentElement.InsertBefore(xmlFragment.FirstChild.ChildNodes[1], item);
					added = true;
					break;
				}
				else if (compare == 0)
				{
					xmlDoc.DocumentElement.ReplaceChild(xmlFragment.FirstChild.ChildNodes[1], item);
					added = true;
					break;
				}
			}

			if (!added)
			{
				xmlDoc.DocumentElement.AppendChild(xmlFragment.FirstChild.ChildNodes[1]);
				added = true;
			}

			// Save the document to a file and auto-indent the output.
			XmlTextWriter writer = new XmlTextWriter(LibraryFile, null);
			writer.IndentChar = '\t';
			writer.Indentation = 1;
			writer.Formatting = Formatting.Indented;
			xmlDoc.Save(writer);

			//xmlDoc.Save(LibraryFile);
			writer.Close();
			writer = null;
			xmlDoc = null;

			Directory.Delete(Path.GetDirectoryName(FileName), true);
		}
	}
}