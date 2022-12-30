using GameFramework.Fsm;
using GameFramework.Localization;
using GameFramework.Procedure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Xuan{
    public class ProcedureLaunch : ProcedureBase
    {
        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            //发布版本时，将数据写入Assets/GameMain/Configs/BuildInfo.txt，供游戏逻辑读取
            GameEntry.BuiltinData.InitBuildInfo();

            // 语言配置：设置当前使用的语言，如果不设置，则默认使用操作系统语言。
            InitLanguageSettings();

            // 变体配置：根据使用的语言，通知底层加载对应的资源变体
            InitCurrentVariant();

            base.OnEnter(procedureOwner);
        }

        private void InitCurrentVariant()
        {
            if (GameEntry.Base.EditorResourceMode)
            {
                return;
            }

            string currentVariant = null;
            switch (GameEntry.Localization.Language)
            {
                case Language.English:
                    currentVariant = "en-us";
                    break;

                case Language.ChineseSimplified:
                    currentVariant = "zh-cn";
                    break;

                case Language.ChineseTraditional:
                    currentVariant = "zh-tw";
                    break;

                default:
                    currentVariant = "en-us";
                    break;
            }
           //这里通知语言相关资源文件进行对应的加载。
            GameEntry.Resource.SetCurrentVariant(currentVariant);
            Log.Info(" 资源变体设置通知完成，当前语言为:{0}.", currentVariant);
        }

        private void InitLanguageSettings()
        {
            if (GameEntry.Base.EditorResourceMode && GameEntry.Base.EditorLanguage != Language.Unspecified)
            {
                //若使用编辑器资源模式，直接使用base面板上设置的语言
                Language language = (Language)GameEntry.Setting.GetInt(Constant.Setting.Language, (int)Language.English);
                if (language != Language.English
               && language != Language.ChineseSimplified
               && language != Language.ChineseTraditional)
                {
                    // 若是暂不支持的语言，则使用英语
                    language = Language.English;

                    GameEntry.Setting.SetString(Constant.Setting.Language, language.ToString());
                    GameEntry.Setting.Save();
                }
                GameEntry.Localization.Language = language;
                Log.Info("编辑器下语言设置完成，当前语言为{0}", language.ToString());
            }
        }
    }
}
