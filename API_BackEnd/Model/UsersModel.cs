﻿namespace API_BackEnd.Model
{
    public class UsersModel
    {   public string Id {  get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }
    }
}
