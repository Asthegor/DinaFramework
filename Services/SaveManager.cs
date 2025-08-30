using System;
using System.IO;
using System.Text.Json;

namespace DinaFramework.Services
{
    /// <summary>
    /// Permet de sauvegarder ou charger un objet vers ou depuis un fichier crypté.
    /// </summary>
    public static class SaveManager
    {
        /// <summary>
        /// Charge un objet depuis un fichier crypté et le désérialise dans le type spécifié.
        /// </summary>
        /// <typeparam key="T">Le type de l'objet à charger.</typeparam>
        /// <param key="filePath">Le chemin du fichier crypté.</param>
        /// <returns>L'objet désérialisé, ou la valeur par défaut si le fichier n'existe pas ou est vide.</returns>
        public static T? LoadObjectFromEncryptFile<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string encryptString = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(encryptString))
                    return default;
                string jsonString = DLACryptographie.EncryptDecrypt.DecryptText(encryptString);
                return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);
            }

            return default;
        }
        /// <summary>
        /// Sauvegarde un objet dans un fichier en format crypté, avec possibilité de remplacer le fichier existant.
        /// </summary>
        /// <typeparam key="T">Le type de l'objet à sauvegarder.</typeparam>
        /// <param key="obj">L'objet à sauvegarder.</param>
        /// <param key="fileFullname">Le chemin complet du fichier.</param>
        /// <param key="overwritten">Indique si le fichier doit être écrasé.</param>
        /// <returns>Vrai si l'objet a été sauvegardé avec succès, sinon faux.</returns>
        public static bool SaveObjectToFile<T>(T obj, string fileFullname, bool overwritten = true)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(obj, _jsonOptions);
                string encryptString = DLACryptographie.EncryptDecrypt.EncryptText(jsonString);

                if (overwritten)
                    File.WriteAllText(fileFullname, encryptString);
                else
                    File.AppendAllText(fileFullname, encryptString);
                return true;
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
            {
                return false;
            }
        }
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
    }
}
