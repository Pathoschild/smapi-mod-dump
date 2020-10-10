/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using SaveAnywhereV3.DataContract;

namespace SaveAnywhereV3.Service
{
    public abstract class SaveLoadServiceBase<TModel> : ISaveLoadService
        where TModel : class
    {
        private readonly Func<AggregatedModel, TModel> m_modelGetter;
        private readonly Action<AggregatedModel, TModel> m_modelSetter;

        protected string SaveFilePath => Global.GetSaveFilePath(this.GetType().Name + ".json");
        protected string SaveFileTempPath => SaveFilePath + ".tmp";

        protected SaveLoadServiceBase(Expression<Func<AggregatedModel, TModel>> modelResolver)
        {
            this.m_modelGetter = modelResolver.Compile();

            var newValue = Expression.Parameter(modelResolver.Body.Type);
            var assign = Expression.Lambda<Action<AggregatedModel, TModel>>(
                Expression.Assign(modelResolver.Body, newValue),
                modelResolver.Parameters[0], newValue);

            this.m_modelSetter = assign.Compile();
        }

        public virtual bool Check()
        {
            return File.Exists(SaveFilePath);
        }

        public virtual void Clear()
        {
            File.Delete(SaveFilePath);
            File.Delete(SaveFileTempPath);
        }

        public void Commit()
        {
            File.Delete(SaveFilePath);
            File.Move(SaveFileTempPath, SaveFilePath);
        }

        public void Load()
        {
            DoLoad(Global.Helper.ReadJsonFile<TModel>(SaveFilePath));
        }

        public void Save()
        {
            Global.Helper.WriteJsonFile(SaveFileTempPath, DumpSaveModel());
        }

        protected abstract TModel DumpSaveModel();

        protected abstract void DoLoad(TModel model);

        public virtual void SaveTo(AggregatedModel model)
        {
            m_modelSetter(model, DumpSaveModel());
        }

        public virtual void LoadFrom(AggregatedModel model)
        {
            DoLoad(m_modelGetter(model));
        }
    }
}
