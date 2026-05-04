using System;
using System.IO;
using Microsoft.Win32;

class SetupFrydia {
	static void Main() {
		string folderPath   = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Frydia";
		string baseDir      = AppDomain.CurrentDomain.BaseDirectory;
		string launchName   = "launcher.exe";
		string binName      = "Frydia.exe";
		string launcherPath = Path.Combine(baseDir, launchName);
		string binaryPath   = Path.Combine(baseDir, binName);

		if (!File.Exists(launcherPath))
		{
			Console.WriteLine("[-] Error: " + launcherPath + " introuvable.");
			return;
		}
		
		if (!File.Exists(binaryPath))
		{
			Console.WriteLine("[-] Error: " + binaryPath + " introuvable.");
			return;
		}

		if (!Directory.Exists(folderPath)) {
			Directory.CreateDirectory(folderPath);
			Console.WriteLine("[+] Fichier de destination " + folderPath + " crée.");
		}
		
		string destinationPath = Path.Combine(folderPath, launchName);
		File.Copy(launcherPath, destinationPath, true);
		
		destinationPath = Path.Combine(folderPath, binName);
		File.Copy(binaryPath, destinationPath, true);
		Console.WriteLine("[+] Fichiers copié avec succès.");
		
		RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
		key.SetValue("Frydia", "\"" + Path.Combine(folderPath, launchName) + "\"");
		key.Close();
		Console.WriteLine("[+] Fichiers rendu persistant.");
		
		System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
		{
			FileName = Path.Combine(folderPath, launchName),
			WorkingDirectory = folderPath,
			UseShellExecute = true
		});
		Console.WriteLine("[+] Programme lancé en arrière plan.");
		Console.WriteLine("\n[+] Setup terminé avec succès.");
		
		System.Threading.Thread.Sleep(500);
		Environment.Exit(0);
	}
}