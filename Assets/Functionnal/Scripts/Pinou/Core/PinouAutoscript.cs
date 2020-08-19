using Pinou.InputSystem;
using System.IO;
using System.Text;
using UnityEngine;

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
        private static void SceneInfos_AppendAllHolders(StringBuilder enumBuilder, StringBuilder fieldBuilder, StringBuilder switchBuilder, PinouSceneInfosData.SceneHolderParams holder)
        {
            AppendHolder(enumBuilder, fieldBuilder, switchBuilder, holder);
            if (holder.SubHolders == null) { holder.SubHolders = new PinouSceneInfosData.SceneHolderParams[] { }; }
            for (int i = 0; i < holder.SubHolders.Length; i++)
            {
                SceneInfos_AppendAllHolders(enumBuilder, fieldBuilder, switchBuilder, holder.SubHolders[i]);
            }
        }
        private static void AppendHolder(StringBuilder enumBuilder, StringBuilder fieldBuilder, StringBuilder switchBuilder, PinouSceneInfosData.SceneHolderParams holder)
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


        public static void UpdateBeingResourcesAutoScript(string entityBeingDataPath, string entityEnumsPath, string[] beingBaseEnumLines, string[] resourcesNames)
        {
            string entityBeingDataTemplatePath = Directory.GetCurrentDirectory() + "\\" + entityBeingDataPath + "EntityBeingData_AutoscriptTemplate";
            entityBeingDataPath = Directory.GetCurrentDirectory() + "\\" + entityBeingDataPath + "EntityBeingData_Autoscript.cs";

            string entityEnumsTemplatePath = Directory.GetCurrentDirectory() + "\\" + entityEnumsPath + "EntityEnums_AutoscriptTemplate";
            entityEnumsPath = Directory.GetCurrentDirectory() + "\\" + entityEnumsPath + "EntityEnums_Autoscript.cs";

            string beingDataAutoscript = File.ReadAllText(entityBeingDataTemplatePath);
            string entityEnumsAutoscript = File.ReadAllText(entityEnumsTemplatePath);

            #region EnumBuilder
            StringBuilder strBuilder = new StringBuilder();
            int powCount = -1;
            for (int i = 0; i < beingBaseEnumLines.Length; i++)
            {
                strBuilder.Append("        ");
                strBuilder.Append(beingBaseEnumLines[i]);
                strBuilder.Append(" = ");
                strBuilder.Append(Mathf.Floor(Mathf.Pow(2, powCount++)));
                strBuilder.Append(",");

                if (i < beingBaseEnumLines.Length - 1)
                    strBuilder.Append("\n");
            }
            entityEnumsAutoscript = entityEnumsAutoscript.Replace("|BASE|", strBuilder.ToString());
            strBuilder.Length = 0;
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("\n");
                strBuilder.Append("        Max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" = ");
                strBuilder.Append(Mathf.Floor(Mathf.Pow(2, powCount++)));
                strBuilder.Append(",\n");
                strBuilder.Append("        ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("Regen = ");
                strBuilder.Append(Mathf.Floor(Mathf.Pow(2, powCount++)));
                strBuilder.Append(",\n");
                strBuilder.Append("        ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("ReceivedFactor = ");
                strBuilder.Append(Mathf.Floor(Mathf.Pow(2, powCount++)));
                strBuilder.Append(",\n");
                strBuilder.Append("        ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("DoneFactor = ");
                strBuilder.Append(Mathf.Floor(Mathf.Pow(2, powCount++)));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append(",\n");
            }
            if (powCount >= 32)
            {
                throw new System.Exception("Extreme fatal error. Too much enum values !!! Maybe try something with enum long blablabla.");
            }
            entityEnumsAutoscript = entityEnumsAutoscript.Replace("|RESOURCES|", strBuilder.ToString());
            strBuilder.Length = 0;
            powCount = 0;
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("        ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" = ");
                strBuilder.Append(Mathf.Floor(Mathf.Pow(2, powCount++)));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append(",\n");
            }
            entityEnumsAutoscript = entityEnumsAutoscript.Replace("|TYPES|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Fields
            #region Data protected fields
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
                strBuilder.Append("				case EntityBeingResourcesType.");
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
                strBuilder.Append("				case EntityBeingResourcesType.");
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
                strBuilder.Append("				case EntityBeingResourcesType.");
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
                strBuilder.Append(string.Format("{1}case EntityBeingResourcesType.{0}:\n{1}	return startNotAtMax{0};", resourcesNames[i], "				"));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHSTARTNOTATMAXDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Data valueStartAmount switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourcesType.{0}:\n{1}	return start{0};", resourcesNames[i], "				"));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHSTARTAMOUNTDATA|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Instance protected fields
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("			//");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("\n			protected float current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";\n\n			public float Current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" => current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";\n			public float ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("Regen => _data.");
                strBuilder.Append(resourcesNames[i].ToLower());
                strBuilder.Append("Regen;\n			public float ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("ReceiveFactor => _data.");
                strBuilder.Append(resourcesNames[i].ToLower());
                strBuilder.Append("ReceiveFactor;\n			public float Max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" => _data.max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";\n			public float ");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("Progress => current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" / _data.max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";\n\n			public void Set");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("(float value)\n			{\n				current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" = Mathf.Clamp(value, 0f, _data.max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(");\n			}\n\n			public void Modify");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("(float amount)\n			{\n				current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" += amount;\n				current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" = Mathf.Clamp(current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(", 0f, _data.max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(");\n			}");

                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|RESOURCESINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;

            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("			public void SetResource");
                strBuilder.Append("(float value)\n			{\n				current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" = Mathf.Clamp(value, 0f, _data.max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(");\n			}\n\n			public void Modify");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append("(float amount)\n			{\n				current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" += amount;\n				current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(" = Mathf.Clamp(current");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(", 0f, _data.max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(");\n			}");

                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|RESOURCESINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Instance GetCurrent switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourcesType.{0}:\n{1}	return current{0};", resourcesNames[i], "                    "));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHGETCURRENTINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance SetCurrent switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourcesType.{0}:\n{1}	current{0} = value;\n{1}	current{0} = Mathf.Clamp(current{0}, 0f, _data.max{0});\n{1}	break;", resourcesNames[i], "                    "));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHSETCURRENTINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance ModifyCurrent switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append(string.Format("{1}case EntityBeingResourcesType.{0}:\n{1}	current{0} += amount;\n{1}	current{0} = Mathf.Clamp(current{0}, 0f, _data.max{0});\n{1}	break;", resourcesNames[i], "                    "));
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHMODIFYCURRENTINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion

            #region Instance maxValue switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("					case EntityBeingResourcesType.");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(":\n						return _data.max");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(";");
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHMAXINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance valueRegen switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("					case EntityBeingResourcesType.");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(":\n						return _data.");
                strBuilder.Append(resourcesNames[i].ToLower());
                strBuilder.Append("Regen;");
                if (i < resourcesNames.Length - 1)
                    strBuilder.Append("\n");
            }
            beingDataAutoscript = beingDataAutoscript.Replace("|SWITCHREGENINSTANCE|", strBuilder.ToString());
            strBuilder.Length = 0;
            #endregion
            #region Instance valueReceiveFactor switch
            for (int i = 0; i < resourcesNames.Length; i++)
            {
                strBuilder.Append("					case EntityBeingResourcesType.");
                strBuilder.Append(resourcesNames[i]);
                strBuilder.Append(":\n						return _data.");
                strBuilder.Append(resourcesNames[i].ToLower());
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

            if (File.Exists(entityEnumsPath) == false)
            {
                File.Create(entityEnumsPath);
            }
            if (entityEnumsAutoscript.Equals(File.ReadAllText(entityEnumsPath)) == false)
            {
                File.WriteAllText(entityEnumsPath, entityEnumsAutoscript);
            }
        }


    }
}