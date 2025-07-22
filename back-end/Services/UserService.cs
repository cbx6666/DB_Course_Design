using BackEnd.Dtos;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        // ͨ�����캯��ע��ִ���
        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            // ���òִ���Ļ�ȡ�����û����ݵĲ���
            var users = await _repo.GetAllAsync();

            // ��ʵ����ӳ��Ϊ DTO������ȡ��Ҫ������ǰ�˵�����
            return users.Select(u => new UserDto
            {
                UserID = u.UserID,
                Username = u.Username,
                Email = u.Email
            });
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            // ���òִ����ָ���û����ݵĲ���
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                AccountCreationTime = DateTime.UtcNow
            };

            // �ڲִ�������û��Ĳ���
            await _repo.AddAsync(user);
            // �ִ�����б���Ĳ���
            await _repo.SaveAsync();

            return new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<bool> UpdateUserAsync(int id, CreateUserDto dto)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return false;

            // �޸Ĳֿ������
            user.Username = dto.Username;
            user.Password = dto.Password;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            await _repo.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return false;

            // ɾ���ֿ��ָ���û��Ĳ���
            await _repo.DeleteAsync(user);
            await _repo.SaveAsync();
            return true;
        }
    }
}
