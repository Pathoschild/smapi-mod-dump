
using System;
using NUnit.Framework;
using System.IO;

namespace CustomEmojis.Tests {

	[TestFixture]
	public class CustomEmojisTest {

		[TestCase(@"", @"")]
		[TestCase(@"c:\", @"")]
		[TestCase(@"", @"c:\")]
		public void GetRelativePathNullOrEmptyException(string sourceDir, string targetPath) {
			Assert.That(() => GetRelativePath(sourceDir, targetPath), Throws.TypeOf<ArgumentNullException>());
		}

		[Test(Description = "Assert that GetRelativePath returns the expected values.")]
#if true
		[TestCase(@"C:\", @"C:\", ExpectedResult = "./")]
		[TestCase(@"C:\grandparent\parent\child", @"C:\grandparent\parent\sibling", ExpectedResult = @"..\sibling")]
		[TestCase(@"C:\grandparent\parent\child", @"C:\cousin\file.exe", ExpectedResult = @"..\..\..\cousin\file.exe")]
		[TestCase(@"C:\grandparent\parent\child", @"C:\cousin\", ExpectedResult = @"..\..\..\cousin")]
		[TestCase(@"C:\grandparent\parent\", @"C:\cousin\", ExpectedResult = @"..\..\cousin")]
#else
		[TestCase("/", "/", ExpectedResult = "./")]
		[TestCase("/grandparent/parent/child", "/grandparent/parent/sibling", ExpectedResult = "../sibling")]
		[TestCase("/grandparent/parent/child", "/cousin/file.exe", ExpectedResult = "../../../cousin/file.exe")]
#endif
		public string GetRelativePath(string sourceDir, string targetPath) {

			if(String.IsNullOrEmpty(sourceDir)) {
				throw new ArgumentNullException("fromPath");
			}

			if(String.IsNullOrEmpty(targetPath)) {
				throw new ArgumentNullException("toPath");
			}

			if(sourceDir.EndsWith("\\")) {
				sourceDir = sourceDir.TrimEnd('\\');
			}

			sourceDir += Path.DirectorySeparatorChar;

			if(targetPath.EndsWith("\\")) {
				targetPath = targetPath.TrimEnd('\\') + Path.DirectorySeparatorChar;
			}

			Uri fromUri = new Uri(sourceDir);
			Uri toUri = new Uri(targetPath);

			if(fromUri.Scheme != toUri.Scheme) { // path can't be made relative.
				return targetPath;
			}

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if(relativePath == "") {
				relativePath = "./";
			} else if(toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase)) {
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath.TrimEnd('\\');
		}

		[TestCase(@"C:\", @"C:\", ExpectedResult = "./")]
		[TestCase(@"C:\grandparent\parent\child", @"C:\grandparent\parent\sibling", ExpectedResult = @"..\sibling")]
		[TestCase(@"C:\grandparent\parent\child", @"C:\cousin\file.exe", ExpectedResult = @"..\..\..\cousin\file.exe")]
		[TestCase(@"C:\grandparent\parent\child", @"C:\cousin\", ExpectedResult = @"..\..\..\cousin")]
		[TestCase(@"C:\grandparent\parent\", @"C:\cousin\", ExpectedResult = @"..\..\cousin")]
		//[TestCase(@"", @"", ExpectedResult = @"")]
		public string GetRelativePath2(string folder, string filespec) {
			Uri pathUri = new Uri(filespec);
			// Folders must end in a slash
			if(!folder.EndsWith(Path.DirectorySeparatorChar.ToString())) {
				folder += Path.DirectorySeparatorChar;
			}
			Uri folderUri = new Uri(folder);
			string relativePath = Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
			if(relativePath == "") {
				relativePath = "./";
			}
			return relativePath.TrimEnd('\\');
		}

	}

}
