using Manager.Domain.Validators;

namespace Manager.Domain.Entities
{
    public class User : Base
    {
        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;

            _errors = [];
        }

        //EF
        protected User() { }

        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        public override bool Validate()
        {
            var validationResult = new UserValidator().Validate(this);
            if (!validationResult.IsValid)
            {
                string erros = string.Join(" ", validationResult.Errors.Select(x => x.ErrorMessage));
                throw new ArgumentException("Alguns erros ocorreram: " + erros);
            }

            return true;
        }

        public void ChangeName(string name)
        {
            Name = name;
            Validate();
        }

        public void ChangePassword(string password)
        {
            Password = password;
            Validate();
        }

        public void ChangeEmail(string email)
        {
            Email = email;
            Validate();
        }
    }
}
