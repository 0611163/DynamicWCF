using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFCommon
{
    /// <summary>
    /// 服务接口
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// 服务启动
        /// </summary>
        void OnServiceStart();

        /// <summary>
        /// 服务停止
        /// </summary>
        void OnServiceStop();
    }
}
