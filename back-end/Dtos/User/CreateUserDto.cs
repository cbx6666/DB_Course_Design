using System;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Dtos.User
{
	// ���������û�ʱ�ύ������
	public class CreateUserDto
	{
		[Required]
		[MaxLength(15)]
		public string Username { get; set; } = null!;

		[Required]
		[MaxLength(10)]
		public string Password { get; set; } = null!;

		[Required]
		public long PhoneNumber { get; set; }

		[Required]
		[MaxLength(30)]
		public string Email { get; set; } = null!;
	}
}
