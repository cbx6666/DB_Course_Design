using BackEnd.Dtos;

namespace BackEnd.Services.Interfaces
{
    public interface IUserService
    {
        // ��ȡ�����û�
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        // ����ID��ȡ�û�
        Task<UserDto?> GetUserByIdAsync(int id);
        // �������û�
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        // �����û�
        Task<bool> UpdateUserAsync(int id, CreateUserDto dto);
        // ɾ���û�
        Task<bool> DeleteUserAsync(int id);
    }
}
