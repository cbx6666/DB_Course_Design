using BackEnd.DTOs.AfterSaleApplication;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 创建售后申请服务接口
    /// </summary>
    public interface ICreateApplicationService
    {
        /// <summary>
        /// 创建售后申请
        /// </summary>
        /// <param name="request">创建申请请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>创建结果</returns>
        Task<CreateApplicationResult> CreateApplicationAsync(CreateApplicationDto request, int userId);
    }
}
