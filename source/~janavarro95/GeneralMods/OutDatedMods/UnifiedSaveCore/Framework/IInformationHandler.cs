using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedSaveCore.Framework
{
    public interface IInformationHandler
    {

        void beforeSave();
        void afterSave();
        void afterLoad();
    }
}
