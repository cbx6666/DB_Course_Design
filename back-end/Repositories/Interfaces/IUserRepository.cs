using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // ��ȡ�����û�
        Task<IEnumerable<User>> GetAllAsync();
        // �����û�ID��ȡ�û�
        Task<User?> GetByIdAsync(int id);
        // ���һ�����û�
        Task AddAsync(User user);
        // ����ָ���û���Ϣ
        Task<bool> UpdateUserAsync(User user);
        // ɾ��ָ��ID���û���
        Task DeleteAsync(User user);
        // �������
        Task SaveAsync();
    }
}
