using Pinou.InputSystem;
using System.IO;
using System.Text;
using UnityEngine;
using Pinou.EntitySystem;
using Pinou.Networking;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using Pinou.Editor;
#endif

namespace Pinou
{
    public static class PinouAutoscript
    {
        public static void UpdateInputManagerAutoscripts(string autoScriptPath, PinouInputParameters[] gameInputs, PinouAxisParameters[] gameAxes)
        {
            string templatePath = Directory.GetCurrentDirectory() + "\\" + autoScriptPath + "PinouInputManager_AutoscriptTemplate";
            autoScriptPath = Directory.GetCurrentDirectory() + "\\" + autoScriptPath + "PinouInputManager_Autoscript.cs";

            string autoScript = File.ReadAllText(templatePath);

            #region Inputs
            StringBuilder enumBuilder = new StringBuilder();
            //StringBuilder swtichBuilder = new StringBuilder();
            for (int i = 0; i < gameInputs.Length; i++)
            {
                enumBuilder.Append("		");
                enumBuilder.Append(gameInputs[i].InputName);
                //swtichBuilder.Append("				case PinouInput.");
                //swtichBuilder.Append(gameInputs[i].InputName);
                //swtichBuilder.Append(":\r\n				    return ");
                //swtichBuilder.Append(i);
                //swtichBuilder.Append(";\r\n");


                if (i < gameInputs.Length - 1)
                {
                    enumBuilder.Append(",\r\n");
                }
            }
            autoScript = autoScript.Replace("|ENUMINPUT|", enumBuilder.ToString());
            //autoScript = autoScript.Replace("|SWITCHINPUT|", swtichBuilder.ToString());
            #endregion
            #region Axes
            enumBuilder.Length = 0;
            //swtichBuilder.Length = 0;
            StringBuilder enumBuilder2 = new StringBuilder();
            //StringBuilder swtichBuilder2 = new StringBuilder();
            for (int i = 0; i < gameAxes.Length; i++)
            {
                if (gameAxes[i].Is2D)
                {
                    enumBuilder2.Append("		");
                    enumBuilder2.Append(gameAxes[i].AxisName);
                    enumBuilder2.Append(" = ");
                    enumBuilder2.Append(i);
                    //swtichBuilder2.Append("				case Pinou2DAxis.");
                    //swtichBuilder2.Append(gameAxes[i].AxisName);
                    //swtichBuilder2.Append(":\r\n				    return ");
                    //swtichBuilder2.Append(i);
                    //swtichBuilder2.Append(";\r\n");
                    if (i < gameAxes.Length - 1)
                    {
                        enumBuilder2.Append(",\r\n");
                    }
                }
                else
                {
                    enumBuilder.Append("		");
                    enumBuilder.Append(gameAxes[i].AxisName);
                    enumBuilder.Append(" = ");
                    enumBuilder.Append(i);
                    //swtichBuilder.Append("				case PinouAxis.");
                    //swtichBuilder.Append(gameAxes[i].AxisName);
                    //swtichBuilder.Append(":\r\n				    return ");
                    //swtichBuilder.Append(i);
                    //swtichBuilder.Append(";\r\n");
                    if (i < gameAxes.Length - 1)
                    {
                        enumBuilder.Append(",\r\n");
                    }
                }
            }
            autoScript = autoScript.Replace("|ENUMAXIS|", enumBuilder.ToString());
            //autoScript = autoScript.Replace("|SWITCHAXIS|", swtichBuilder.ToString());
            autoScript = autoScript.Replace("|ENUMAXIS2D|", enumBuilder2.ToString());
            //autoScript = autoScript.Replace("|SWITCHAXIS2D|", swtichBuilder2.ToString());
            #endregion




            if (File.Exists(autoScriptPath) == false)
            {
                File.Create(autoScriptPath);
            }
            else if (autoScript.Equals(File.ReadAllText(autoScriptPath)))
            {
                return;
            }
            File.WriteAllText(autoScriptPath, autoScript);
        }

		public static void UpdateInputReceiverAutoScript(string autoScriptPath, PinouInputParameters[] gameInputs, PinouAxisParameters[] gameAxes)
        {
            string templatePath = Directory.GetCurrentDirectory() + "\\" + autoScriptPath + "PinouInputReceiver_AutoscriptTemplate";
            autoScriptPath = Directory.GetCurrentDirectory() + "\\" + autoScriptPath + "PinouInputReceiver_Autoscript.cs";

            string autoScript = File.ReadAllText(templatePath);

            #region Inputs
            StringBuilder functionsBuilder = new StringBuilder();
            for (int i = 0; i < gameInputs.Length; i++)
            {
                functionsBuilder.Append("		public bool ");
                functionsBuilder.Append(gameInputs[i].InputName);
                functionsBuilder.Append(" => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.");
                functionsBuilder.Append(gameInputs[i].InputName);
                functionsBuilder.Append(") : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.");
                functionsBuilder.Append(gameInputs[i].InputName);
                functionsBuilder.Append(")) : false;\r\n");
            }
            #endregion
            #region Axes
            StringBuilder functionsBuilder2 = new StringBuilder();
            for (int i = 0; i < gameAxes.Length; i++)
            {
                if (gameAxes[i].Is2D)
                {
                    functionsBuilder2.Append("		public Vector2 ");
                    functionsBuilder2.Append(gameAxes[i].AxisName);
                    functionsBuilder2.Append(" => IsFocused ? (AbsoluteFocus ? Vector2.zero : PinouApp.Input.GameAxis2DValue(_focusingPlayer.ControllerId, Pinou2DAxis.");
                    functionsBuilder2.Append(gameAxes[i].AxisName);
                    functionsBuilder2.Append(")) : Vector2.zero;\r\n");
                }
                else
                {
                    functionsBuilder.Append("		public float ");
                    functionsBuilder.Append(gameAxes[i].AxisName);
                    functionsBuilder.Append(" => IsFocused ? (AbsoluteFocus ? 0f : PinouApp.Input.GameAxisValue(_focusingPlayer.ControllerId, PinouAxis.");
                    functionsBuilder.Append(gameAxes[i].AxisName);
                    functionsBuilder.Append(")) : 0f;\r\n");
                }
            }
            functionsBuilder.Append(functionsBuilder2);
            #endregion

            autoScript = autoScript.Replace("|FUNCTIONS|", functionsBuilder.ToString());



            if (File.Exists(autoScriptPath) == false)
            {
                File.Create(autoScriptPath);
            }
            else if (autoScript.Equals(File.ReadAllText(autoScriptPath)))
            {
                return;
            }
            File.WriteAllText(autoScriptPath, autoScript);
        }

        public static void UpdateSceneInfosAutoScript(string autoScriptPath, PinouSceneInfosData.SceneHolderParams[] sceneHolders)
        {
            string templatePath = Directory.GetCurrentDirectory() + "\\" + autoScriptPath + "PinouSceneInfos_AutoscriptTemplate";
            autoScriptPath = Directory.GetCurrentDirectory() + "\\" + autoScriptPath + "PinouSceneInfos_Autoscript.cs";

            string autoScript = File.ReadAllText(templatePath);

            StringBuilder enumBuilder = new StringBuilder();
            StringBuilder fieldBuilder = new StringBuilder();
            StringBuilder switchBuilder = new StringBuilder();
            for (int i = 0; i < sceneHolders.Length; i++)
            {
                SceneInfos_AppendAllHolders(enumBuilder, fieldBuilder, switchBuilder, sceneHolders[i]);
            }
            autoScript = autoScript.Replace("|ENUM|", enumBuilder.ToString());
            autoScript = autoScript.Replace("|FIELDS|", fieldBuilder.ToString());
            autoScript = autoScript.Replace("|SWITCH|", switchBuilder.ToString());

            if (File.Exists(autoScriptPath) == false)
            {
                File.Create(autoScriptPath);
            }
            else if (autoScript.Equals(File.ReadAllText(autoScriptPath)))
            {
                return;
            }
            File.WriteAllText(autoScriptPath, autoScript);
        }
        private static void SceneInfos_AppendAllHolders(StringBuilder enumBuilder, StringBuilder fieldBuilder, StringBuilder switchBuilder, PinouSceneInfosData.ISubHolder holder)
        {
            AppendHolder(enumBuilder, fieldBuilder, switchBuilder, holder);
            for (int i = 0; i < holder.SubHolders.Count; i++)
            {
                SceneInfos_AppendAllHolders(enumBuilder, fieldBuilder, switchBuilder, holder.SubHolders.ElementAt(i));
            }
        }
        private static void AppendHolder(StringBuilder enumBuilder, StringBuilder fieldBuilder, StringBuilder switchBuilder, PinouSceneInfosData.ISubHolder holder)
        {
            enumBuilder.Append("		");
            enumBuilder.Append(holder.HolderName);
            enumBuilder.Append(",\r\n");

            fieldBuilder.Append("		[SerializeField] private Transform _");
            fieldBuilder.Append(holder.HolderName.ToLower());
            fieldBuilder.Append("Holder;\r\n		public Transform ");
            fieldBuilder.Append(holder.HolderName);
            fieldBuilder.Append("Holder => _");
            fieldBuilder.Append(holder.HolderName.ToLower());
            fieldBuilder.Append("Holder;\r\n");

            switchBuilder.Append("				case SceneHolder.");
            switchBuilder.Append(holder.HolderName);
            switchBuilder.Append(":\r\n					return _");
            switchBuilder.Append(holder.HolderName.ToLower());
            switchBuilder.Append("Holder;\r\n");
        }

        public static void UpdatePinouAppAutoScript(string autoScriptPath, PinouApp.ManagerParameters[] managers)
        {
            string templatePath = Directory.GetCurrentDirectory() + "\\" + autoScriptPath + "PinouApp_AutoscriptTemplate";
            autoScriptPath = Directory.GetCurrentDirectory() + "\\" + autoScriptPath + "PinouApp_Autoscript.cs";

            string autoScript = File.ReadAllText(templatePath);

            #region Inputs
            StringBuilder fieldsBuilder = new StringBuilder();
            for (int i = 0; i < managers.Length; i++)
            {
                if (managers[i].ManagerData == null || managers[i].ManagerVariableName.Length == 0)
                {
                    continue;   
                }

                PinouManagerData.PinouManager manager = managers[i].ManagerData.BuildManagerInstance();
                fieldsBuilder.Append("		private static ");
                fieldsBuilder.Append(manager.GetType().FullName.Replace("+", "."));
                fieldsBuilder.Append(" _");
                fieldsBuilder.Append(managers[i].ManagerVariableName);
                fieldsBuilder.Append(";\r\n		public static ");
                fieldsBuilder.Append(manager.GetType().FullName.Replace("+", "."));
                fieldsBuilder.Append(" ");
                fieldsBuilder.Append(managers[i].ManagerVariableName[0].ToString().ToUpper());
                fieldsBuilder.Append(managers[i].ManagerVariableName.Substring(1));
                fieldsBuilder.Append("=> _");
                fieldsBuilder.Append(managers[i].ManagerVariableName);
                fieldsBuilder.Append(";\r\n");
            }
            #endregion

            autoScript = autoScript.Replace("|FIELDS|", fieldsBuilder.ToString());

            if (File.Exists(autoScriptPath) == false)
            {
                File.Create(autoScriptPath);
            }
            else if (autoScript.Equals(File.ReadAllText(autoScriptPath)))
            {
                return;
            }

            File.WriteAllText(autoScriptPath, autoScript);
        }
        public static void UpdatePinouResourcesDatabasesAutoScript(string fodlerPath, ICollection<string> databasesNames)
        {
            string enumTemplatePath = Directory.GetCurrentDirectory() + "\\" + fodlerPath + "PinouResourcesDatabasesTypes_AutoscriptTemplate";
            string enumPath = Directory.GetCurrentDirectory() + "\\" + fodlerPath + "PinouResourcesDatabasesTypes_Autoscript.cs";

            string enumAutoscript = File.ReadAllText(enumTemplatePath);

            #region EnumBuilder
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < databasesNames.Count; i++)
            {
                strBuilder.Append("        ");
                strBuilder.Append(databasesNames.ElementAt(i));
                strBuilder.Append(" = ");
                strBuilder.Append(i);
                if (i < databasesNames.Count - 1)
                    strBuilder.Append(",\n");
            }
            enumAutoscript = enumAutoscript.Replace("|ENUM|", strBuilder.ToString());
            #endregion

            if (File.Exists(enumPath) == false)
            {
                File.Create(enumPath);
            }
            if (enumAutoscript.Equals(File.ReadAllText(enumPath)) == false)
            {
                File.WriteAllText(enumPath, enumAutoscript);
            }
        }


        public static void UpdateBeingResourcesAutoScript(string entityBeingDataPath, string[] resourcesNames)
        {
            string entityBeingDataTemplatePath = Directory.GetCurrentDirectory() + "\\" + entityBeingDataPath + "EntityBeingData_AutoscriptTemplate";
            entityBeingDataPath = Directory.GetCurrentDirectory() + "\\" + entityBeingDataPath + "EntityBeingData_Autoscript.cs";

            string beingDataAutoscript = File.ReadAllText(entityBeingDataTemplatePath);

            #region Fields
            #region Data protected fields
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("        [Header(\"");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("\")]\n        [FoldoutGroup(\"Being Resources\"), SerializeField] protected float max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";\n");
                strBuilder.Append("        [FoldoutGroup(\"Being Resources\"), SerializeField] protected float ");
                strBuilder.Append(resourcesNames[i].ToLower());
                strBuilder.Append("Regen;\n");
                strBuilder.Append("        [FoldoutGroup(\"Being Resources\"), SerializeField] protected float ");
                strBuilder.Append(resourcesNames[i].ToLower());
                strBuilder.Append("ReceiveFactor = 1f;\n");
                strBuilder.Append("        [FoldoutGroup(\"Being Resources\"), SerializeField] protected bool startNotAtMax");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";\n");
                strBuilder.Append("        [FoldoutGroup(\"Being Resources\"), SerializeField, Min(0f), PropertyRange(0f, \"@max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("\"), ShowIf(\"startNotAtMax");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("\")] protected float start");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";\n");
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|RESOURCESFIELDSDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Data maxValue switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("				case EntityBeingResourceType.");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(":\n					return max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";");
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHMAXDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Data valueRegen switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("				case EntityBeingResourceType.");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(":\n					return ");
                strBuilder.Append(resourcesNames[i].ToLower());
                strBuilder.Append("Regen;");
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHREGENDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Data valueReceiveFactor switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("				case EntityBeingResourceType.");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(":\n					return ");
                strBuilder.Append(resourcesNames[i].ToLower());
                strBuilder.Append("ReceiveFactor;");
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHRECEIVEDDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Data valueStartNotAtMax switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourceType.{0}:\n{1}	return startNotAtMax{0};", resourcesNames[i], "				"));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHSTARTNOTATMAXDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Data valueStartAmount switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourceType.{0}:\n{1}	return start{0};", resourcesNames[i], "				"));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHSTARTAMOUNTDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Instance protected fields
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("			//{0}\n			protected float current{0};\n\n			public float Current{0} => current{0};\n", resourcesNames[i], resourcesNames[i].ToLower()));
                strBuilder.Append(string.Format("			public float {0}ReceiveFactor => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.{0}, EntityBeingResourceStat.ResourceReceivedFactor, _data.{1}ReceiveFactor) : _data.{1}ReceiveFactor;\n", resourcesNames[i], resourcesNames[i].ToLower()));
                strBuilder.Append(string.Format("			public float Max{0} => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.{0}, EntityBeingResourceStat.MaxResource, _data.max{0}) : _data.max{0};\n", resourcesNames[i]));
                strBuilder.Append(string.Format("			public float {0}Progress => current{0} / Max{0};\n\n", resourcesNames[i]));
                strBuilder.Append(string.Format("			public void Set{0}(float value)\n			{{\n				current{0} = Mathf.Clamp(value, 0f, Max{0});\n			}}\n\n", resourcesNames[i]));
                strBuilder.Append(string.Format("			public void Modify{0}(float amount)\n			{{\n				current{0} += amount;\n				current{0} = Mathf.Clamp(current{0}, 0f, Max{0});\n			}}", resourcesNames[i]));

                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|RESOURCESINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Instance GetCurrent switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourceType.{0}:\n{1}	return current{0};", resourcesNames[i], "                    "));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHGETCURRENTINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance SetCurrent switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourceType.{0}:\n{1}	current{0} = value;\n{1}	current{0} = Mathf.Clamp(current{0}, 0f, Max{0});\n{1}	break;", resourcesNames[i], "                    "));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHSETCURRENTINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance ModifyCurrent switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourceType.{0}:\n{1}	current{0} += amount;\n{1}	current{0} = Mathf.Clamp(current{0}, 0f, Max{0});\n{1}	break;", resourcesNames[i], "                    "));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHMODIFYCURRENTINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Instance maxValue switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("					case EntityBeingResourceType.");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(":\n						return Max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";");
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHMAXINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance GetProgress switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourceType.{0}:\n{1}	return {0}Progress;", resourcesNames[i], "                    "));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHGETPROGRESSINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance valueRegen switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                /*strBuilder.Append("					case EntityBeingResourceType.");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(":\n						return ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("Regen;");*/
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            strBuilder.Append("default: return 0f;");
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHREGENINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance valueReceiveFactor switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("					case EntityBeingResourceType.");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(":\n						return ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("ReceiveFactor;");
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHRECEIVEDINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #endregion

            if (File.Exists(entityBeingDataPath) == false)
            {
                File.Create(entityBeingDataPath);
            }
            if (beingDataAutoscript.Equals(File.ReadAllText(entityBeingDataPath)) == false)
            {
                File.WriteAllText(entityBeingDataPath, beingDataAutoscript);
            }
        }
        public static void UpdateStatsLevelsAutoScript(string entityStatsDataPath, EntitiesDynamicConfiguration.LevelTemplateData[] levelTemplates)
        {
            string entityStatsDataTemplatePath = Directory.GetCurrentDirectory() + "\\" + entityStatsDataPath + "EntityStatsData_AutoscriptTemplate";
            entityStatsDataPath = Directory.GetCurrentDirectory() + "\\" + entityStatsDataPath + "EntityStatsData_Autoscript.cs";

            string statsDataAutoscript = File.ReadAllText(entityStatsDataTemplatePath);

            #region Fields
            #region Data protected fields
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("		[SerializeField] protected bool has{0}Level;\n", levelTemplates[i].LevelName));
                strBuilder.Append(string.Format("		[ShowIf(\"@has{0}Level\"), SerializeField] protected LevelData {1}Level;\n", levelTemplates[i].LevelName, levelTemplates[i].LevelName.ToLower()));

                strBuilder.Append(string.Format("		public LevelData {0}Level => {1}Level;\n", levelTemplates[i].LevelName, levelTemplates[i].LevelName.ToLower()));
                strBuilder.Append(string.Format("		public bool Has{0}Level => has{0}Level;\n", levelTemplates[i].LevelName));

                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|LEVELSFIELDSDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Data GetHasLevel switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("				case EntityStatsLevelType.{0}:\n					return has{0}Level;", levelTemplates[i].LevelName));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHGETHASLEVELDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Data GetLevelData switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("				case EntityStatsLevelType.{0}:\n					return {1}Level;", levelTemplates[i].LevelName, levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHGETLEVELDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Instance protected fields
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("			//{0}\n", levelTemplates[i].LevelName));
                strBuilder.Append(string.Format("			protected LevelExperienceData {0}Experience;\n", levelTemplates[i].LevelName.ToLower()));
                strBuilder.Append(string.Format("			public LevelExperienceData {0}Experience => {1}Experience;\n", levelTemplates[i].LevelName, levelTemplates[i].LevelName.ToLower()));
                strBuilder.Append(string.Format("			public bool Has{0}Level => _data.Has{0}Level;\n", levelTemplates[i].LevelName));

                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|LEVELSINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Instance GetLevelDataExperience switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	return {2}Experience;", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHGETLEVELEXPERIENCEDATAINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance SetLevelDataExperience switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	{2}Experience = experienceData;\n{1}	break;", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHSETLEVELEXPERIENCEDATAINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Instance GetCurrentLevel switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	return {2}Experience.Level;", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHGETCURRENTINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance SetCurrentLevel switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	{2}Experience.SetLevel(value);\n{1}	break;", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHSETCURRENTINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance ModifyCurrentLevel switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	{2}Experience.ModifyLevel(amount);\n{1}	break;", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHMODIFYCURRENTINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance GetMaxLevel switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	return {2}Experience.MaxLevel;", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHMAXINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance GetLevelProgress switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	return {2}Experience.ExperienceProgress;", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHGETPROGRESSINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance GetLevelExperience switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {	
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	return {2}Experience.Experience;", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHGETEXPERIENCEINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance SetLevelExperience switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	{2}Experience.SetExperience(experience);\n{1}	break;\n", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHSETEXPERIENCEINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance ModifyLevelExperience switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n", levelTemplates[i].LevelName, "					"));
                strBuilder.Append(string.Format("{0}if (useStatsInfluence == true)\n", "						"));
                strBuilder.Append(string.Format("{0}{1}Experience.ModifyExperience(EvaluateStatsStat(EntityStatsStat.ExperienceGainedGlobalFactor, EvaluateStatsLevelStat(EntityStatsLevelType.{1}, EntityStatsLevelStat.ExperienceGainedFactor, experience)));\n", "							", levelTemplates[i].LevelName));
                strBuilder.Append(string.Format("{0}else\n", "						"));
                strBuilder.Append(string.Format("{0}{1}Experience.ModifyExperience(experience);\n", "							", levelTemplates[i].LevelName.ToLower()));
                strBuilder.Append(string.Format("{0}break;", "						"));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHMODIFYEXPERIENCEINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance SetLevelExperiencePct switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	{2}Experience.SetExperiencePct(experience);\n{1}	break;\n", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHSETPCTEXPERIENCEINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance ModifyLevelExperiencePct switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	{2}Experience.ModifyExperiencePct(experience);\n{1}	break;\n", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHMODIFYPCTEXPERIENCEINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance GetTotalExperienceForNextLevel switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	return {2}Experience.TotalExperienceForNextLevel;\n", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHGETTOTALEXPERIENCEFORNEXTLEVEL|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance GetRemainingExperienceForNextLevel switch
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityStatsLevelType.{0}:\n{1}	return {2}Experience.RemainingExperienceForNextLevel;\n", levelTemplates[i].LevelName, "					", levelTemplates[i].LevelName.ToLower()));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append("\n");
            }
            statsDataAutoscript = statsDataAutoscript.Replace("|SWITCHGETREMAININGEXPERIENCEFORNEXTLEVEL|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion


            #endregion

            if (File.Exists(entityStatsDataPath) == false)
            {
                File.Create(entityStatsDataPath);
            }
            if (statsDataAutoscript.Equals(File.ReadAllText(entityStatsDataPath)) == false)
            {
                File.WriteAllText(entityStatsDataPath, statsDataAutoscript);
            }
        }
        public static void UpdateEquipmentTypes(string entitiesDynamicComponentsPath, string[] equipmentTypes)
        {
            string templatePath = Directory.GetCurrentDirectory() + "\\" + entitiesDynamicComponentsPath + "EntityEquipmentData_AutoscriptTemplate";
            string filePath = Directory.GetCurrentDirectory() + "\\" + entitiesDynamicComponentsPath + "EntityEquipmentData_Autoscript.cs";

            string autoscriptContent = File.ReadAllText(templatePath);

            #region StrBuilder
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < equipmentTypes.Length; i++)
            {
                strBuilder.Append(string.Format("		[SerializeField, ValidateInput(\"Validate{0}Builder\", \"Builder must have {0} built type\"), InlineEditor] protected EntityEquipableBuilder default{0}Builder;\n", equipmentTypes[i]));
                strBuilder.Append(string.Format("		private bool Validate{0}Builder(EntityEquipableBuilder builder) {{ return builder == null || builder.BuiltType == EntityEquipableType.{0}; }}\n", equipmentTypes[i]));
                strBuilder.Append(string.Format("		public bool HasDefault{0} => default{0}Builder != null;\n", equipmentTypes[i]));
                strBuilder.Append(string.Format("		public EntityEquipableBuilder Default{0}Builder => default{0}Builder;\n", equipmentTypes[i]));

                if (i < equipmentTypes.Length - 1)
                    strBuilder.Append("\n");
            }
            autoscriptContent = autoscriptContent.Replace("|PRIVATEBUILDERFIELDS|", strBuilder.ToString());
            strBuilder.Length = 0;

            for (int i = 0; i < equipmentTypes.Length; i++)
            {
                strBuilder.Append(string.Format("				case EntityEquipableType.{0}:\n", equipmentTypes[i]));
                strBuilder.Append(string.Format("					return default{0}Builder;", equipmentTypes[i]));

                if (i < equipmentTypes.Length - 1)
                    strBuilder.Append("\n");
            }
            autoscriptContent = autoscriptContent.Replace("|SWITCHDEFAULTEQUIPMENT|", strBuilder.ToString());
            strBuilder.Length = 0;

            for (int i = 0; i < equipmentTypes.Length; i++)
            {
                strBuilder.Append(string.Format("			protected EntityEquipable equipped{0};\n", equipmentTypes[i]));
                strBuilder.Append(string.Format("			public bool Has{0}Equipped => equipped{0} != null;\n", equipmentTypes[i]));
                strBuilder.Append(string.Format("			public EntityEquipable Equipped{0} => equipped{0};\n", equipmentTypes[i]));

                if (i < equipmentTypes.Length - 1)
                    strBuilder.Append("\n");
            }
            autoscriptContent = autoscriptContent.Replace("|PRIVATEINSTANCEFIELDS|", strBuilder.ToString());
            strBuilder.Length = 0;

            for (int i = 0; i < equipmentTypes.Length; i++)
            {
                strBuilder.Append(string.Format("					case EntityEquipableType.{0}:\n", equipmentTypes[i]));
                strBuilder.Append(string.Format("						return Has{0}Equipped;", equipmentTypes[i]));

                if (i < equipmentTypes.Length - 1)
                    strBuilder.Append("\n");
            }
            autoscriptContent = autoscriptContent.Replace("|SWITCHHASEQUIPPED|", strBuilder.ToString());
            strBuilder.Length = 0;

            for (int i = 0; i < equipmentTypes.Length; i++)
            {
                strBuilder.Append(string.Format("					case EntityEquipableType.{0}:\n", equipmentTypes[i]));
                strBuilder.Append(string.Format("						return equipped{0};", equipmentTypes[i]));

                if (i < equipmentTypes.Length - 1)
                    strBuilder.Append("\n");
            }
            autoscriptContent = autoscriptContent.Replace("|SWITCHEQUIPPED|", strBuilder.ToString());
            strBuilder.Length = 0;

            for (int i = 0; i < equipmentTypes.Length; i++)
            {
                strBuilder.Append(string.Format("					case EntityEquipableType.{0}:\n", equipmentTypes[i]));
                strBuilder.Append(string.Format("						equipped{0} = equipable;\n", equipmentTypes[i]));
                strBuilder.Append("						break;");

                if (i < equipmentTypes.Length - 1)
                    strBuilder.Append("\n");
            }
            autoscriptContent = autoscriptContent.Replace("|SWITCHEQUIP|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            if (File.Exists(filePath) == false)
            {
                File.Create(filePath);
            }
            if (autoscriptContent.Equals(File.ReadAllText(filePath)) == false)
            {
                File.WriteAllText(filePath, autoscriptContent);
            }
        }
		public static void UpdateBodySockets(string entitiesDynamicComponentsPath, string[] bodySockets)
        {
            string templatePath = Directory.GetCurrentDirectory() + "\\" + entitiesDynamicComponentsPath + "EntityBody_AutoscriptTemplate";
            string filePath = Directory.GetCurrentDirectory() + "\\" + entitiesDynamicComponentsPath + "EntityBody_Autoscript.cs";

            string autoscriptContent = File.ReadAllText(templatePath);

            #region StrBuilder
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < bodySockets.Length; i++)
            {
                strBuilder.Append(string.Format("		[SerializeField] protected Transform {0}Socket;\n", bodySockets[i].ToLower()));
                strBuilder.Append(string.Format("		public bool Has{0}Socket => {1}Socket != null;\n", bodySockets[i], bodySockets[i].ToLower()));
                strBuilder.Append(string.Format("		public Transform {0}Socket => {1}Socket;\n", bodySockets[i], bodySockets[i].ToLower() ));

                if (i < bodySockets.Length - 1)
                    strBuilder.Append("\n");
            }
            autoscriptContent = autoscriptContent.Replace("|PRIVATEFIELDS|", strBuilder.ToString());
            strBuilder.Length = 0;

            for (int i = 0; i < bodySockets.Length; i++)
            {
                strBuilder.Append(string.Format("				case EntityBodySocket.{0}:\n", bodySockets[i]));
                strBuilder.Append(string.Format("					return {0}Socket;", bodySockets[i].ToLower()));

                if (i < bodySockets.Length - 1)
                    strBuilder.Append("\n");
            }
            autoscriptContent = autoscriptContent.Replace("|SWITCHSOCKET|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            if (File.Exists(filePath) == false)
            {
                File.Create(filePath);
            }
            if (autoscriptContent.Equals(File.ReadAllText(filePath)) == false)
            {
                File.WriteAllText(filePath, autoscriptContent);
            }
        }
        public static void UpdateDynamicEnums(string fodlerPath, string[] resourcesNames, EntitiesDynamicConfiguration.LevelTemplateData[] levelTemplates, string[] equipmentTypes, string[] bodySockets)
        {
            string enumTemplatePath = Directory.GetCurrentDirectory() + "\\" + fodlerPath + "EntityEnums_AutoscriptTemplate";
            string enumPath = Directory.GetCurrentDirectory() + "\\" + fodlerPath + "EntityEnums_Autoscript.cs";

            string enumAutoscript = File.ReadAllText(enumTemplatePath);

            #region EnumBuilder
            StringBuilder strBuilder = new StringBuilder();
            int powCount = 0;
            strBuilder.Length = 0;
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("        ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" = ");
                strBuilder.Append(Mathf.Floor(Mathf.Pow(2, powCount++)));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append(",\n");
            }
            enumAutoscript = enumAutoscript.Replace("|ENUMRESOURCES|", strBuilder.ToString());

            powCount = 0;
            strBuilder.Length = 0;
            for (int i = 0; i < levelTemplates.Length; i++)
            {
                strBuilder.Append("        ");
                strBuilder.Append(levelTemplates[i].LevelName);
                strBuilder.Append(" = ");
                strBuilder.Append(Mathf.Floor(Mathf.Pow(2, powCount++)));
                if (i < levelTemplates.Length - 1)
                    strBuilder.Append(",\n");
            }
            enumAutoscript = enumAutoscript.Replace("|ENUMLEVELS|", strBuilder.ToString());

            strBuilder.Length = 0;
            for (int i = 0; i < equipmentTypes.Length; i++)
            {
                strBuilder.Append("        ");
                strBuilder.Append(equipmentTypes[i]);
                strBuilder.Append(" = ");
                strBuilder.Append(i + 1);
                if (i < equipmentTypes.Length - 1)
                    strBuilder.Append(",\n");
            }
            enumAutoscript = enumAutoscript.Replace("|ENUMEQUIPMENTS|", strBuilder.ToString());

            strBuilder.Length = 0;
            for (int i = 0; i < bodySockets.Length; i++)
            {
                strBuilder.Append("        ");
                strBuilder.Append(bodySockets[i]);
                strBuilder.Append(" = ");
                strBuilder.Append(i + 1);
                if (i < bodySockets.Length - 1)
                    strBuilder.Append(",\n");
            }
            enumAutoscript = enumAutoscript.Replace("|ENUMSOCKETS|", strBuilder.ToString());
            #endregion

            if (File.Exists(enumPath) == false)
            {
                File.Create(enumPath);
            }
            if (enumAutoscript.Equals(File.ReadAllText(enumPath)) == false)
            {
                File.WriteAllText(enumPath, enumAutoscript);
            }
        }

        public static void UpdateNetworkSyncableVariables(PinouNetworkSyncableVariablesData.VariableData[] variables, string folderPath)
        {
            string templatePath = Directory.GetCurrentDirectory() + "\\" + folderPath + "PinouSyncableVariablesTemplate";
            string scriptPath = Directory.GetCurrentDirectory() + "\\" + folderPath + "PinouSyncableVariables.cs";

            string script = File.ReadAllText(templatePath);

            int pow = 0;
            StringBuilder strBuilder = new StringBuilder();
			for (int i = 0; i < variables.Length; i++)
			{
                strBuilder.Append(string.Format("		{0} = {1},", variables[i].VariableName, Mathf.Pow(2, pow++).ToString()));
                if (i < variables.Length - 1)
                    strBuilder.Append("\n");
			}
            script = script.Replace("|ENUM|", strBuilder.ToString());
            strBuilder.Length = 0;
            for (int i = 0; i < variables.Length; i++)
            {
                strBuilder.Append(string.Format("				case SyncableVariable.{0}:\n", variables[i].VariableName));
                strBuilder.Append(string.Format("					return {0};", variables[i].Channel));
                if (i < variables.Length - 1)
                    strBuilder.Append("\n");
            }
            script = script.Replace("|SWITCH|", strBuilder.ToString());

            if (File.Exists(scriptPath) == false)
            {
                File.Create(scriptPath);
            }
            if (script.Equals(File.ReadAllText(scriptPath)) == false)
            {
                File.WriteAllText(scriptPath, script);
            }
        }

#if UNITY_EDITOR
        public static void E_UpdateSpriteRendererLayerOrderExtender(string extenderPath, string[] layerOrdersNames, int[] layerOrders)
		{
            string extenderTemplatePath = Directory.GetCurrentDirectory() + "\\" + extenderPath + "Editor_SpriteRendererExtender_AutoscriptTemplate";
            extenderPath = Directory.GetCurrentDirectory() + "\\" + extenderPath + "Editor_SpriteRendererExtender.cs";
            string extenderAutoScript = File.ReadAllText(extenderTemplatePath);
            StringBuilder str = new StringBuilder();
			for (int i = 0; i < layerOrdersNames.Length; i++)
			{
                str.Append(string.Format("		[MenuItem(\"CONTEXT/Renderer/OrderInLayer/{0}\")]\n		public static void Set{0}Layer_Rend(MenuCommand command) => UpdateLayerOrder(command, {1});\n", layerOrdersNames[i], layerOrders[i]));
                str.Append(string.Format("		[MenuItem(\"CONTEXT/ParticleSystem/OrderInLayer/{0}\")]\n		public static void Set{0}Layer_PS(MenuCommand command) => UpdateLayerOrder(command, {1});\n", layerOrdersNames[i], layerOrders[i]));
                str.Append(string.Format("		[MenuItem(\"CONTEXT/SortingGroup/OrderInLayer/{0}\")]\n		public static void Set{0}Layer_SG(MenuCommand command) => UpdateLayerOrder(command, {1});\n", layerOrdersNames[i], layerOrders[i]));
                if (i < layerOrdersNames.Length - 1)
				{
                    str.Append("\n");
                }
			}
            extenderAutoScript = extenderAutoScript.Replace("|CONTEXT|", str.ToString());
            if (File.Exists(extenderPath) == false)
            {
                File.Create(extenderPath);
            }

            if (extenderAutoScript.Equals(File.ReadAllText(extenderPath)) == false)
            {
                File.WriteAllText(extenderPath, extenderAutoScript);
            }
        }
#endif
    }
}