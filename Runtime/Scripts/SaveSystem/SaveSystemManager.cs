using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eraflo.Common.ObjectSystem;
using System;
using System.IO;
using Newtonsoft.Json;


namespace Eraflo.Common
{
    public class SaveSystemManager : MonoBehaviour
    {
       

        private JsonSerializerSettings settings = new JsonSerializerSettings() {
            Formatting = Formatting.Indented
        };

        public void SaveToFile(string filePath, Level data)
        {
            try{
                string jsonString = JsonConvert.SerializeObject(data, settings);
                File.WriteAllText(filePath, jsonString);
            } catch(Exception ex){
                Debug.Log("Erreur lors de la sauvegarde : " + ex);
            }
        }

        public Level LoadByFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.Log("Le fichier n'existe pas.");
                    return null;
                }

                string jsonString = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Level>(jsonString, settings) ?? null ;
            }
            catch (Exception ex)
            {
                Debug.Log("Erreur lors du chargement : " + ex);
                return null;
            }

        }
    }
}
