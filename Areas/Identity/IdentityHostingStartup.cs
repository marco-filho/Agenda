using System;
using Agenda.Desktop.Areas.Identity.Data;
using Agenda.Desktop.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Agenda.Desktop.Areas.Identity.IdentityHostingStartup))]
namespace Agenda.Desktop.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AppIdentityContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AppIdentityContextConnection")));

                services.AddDefaultIdentity<AppIdentityUser>(options =>
                    {
                        options.Password.RequireNonAlphanumeric =
                        options.Password.RequireLowercase =
                        options.Password.RequireUppercase = false;
                        options.User.RequireUniqueEmail = true;
                    })
                    .AddErrorDescriber<IdentityErrorDescriberPtBr>()
                    .AddEntityFrameworkStores<AppIdentityContext>();
            });
        }
    }

    internal class IdentityErrorDescriberPtBr : IdentityErrorDescriber
    {
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Falha de concorrência otimista, o objeto foi modificado." }; }

        public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = $"Um erro desconhecido ocorreu." }; }

        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"E-mail '{email}' já está sendo utilizado." }; }

        public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"A permissão '{role}' já está sendo utilizada." }; }

        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"O nome de usuário '{userName}' já está sendo utilizado." }; }

        public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"E-mail '{email}' é inválido." }; }

        public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"A permissão '{role}' é inválida." }; }

        public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "Token inválido." }; }

        public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = nameof(InvalidUserName), Description = $"Login '{userName}' é inválido, pode conter apenas letras ou dígitos." }; }

        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Já existe um usuário com este login." }; }
        
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "Senha incorreta." }; }

        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Senhas devem conter ao menos um digito ('0'-'9')." }; }

        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Senhas devem conter ao menos um caracter em caixa baixa ('a'-'z')." }; }

        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Senhas devem conter ao menos um caracter não alfanumérico." }; }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return base.PasswordRequiresUniqueChars(uniqueChars);
        }

        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Senhas devem conter ao menos um caracter em caixa alta ('A'-'Z')." }; }

        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"Senhas devem conter ao menos {length} caracteres." }; }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return base.RecoveryCodeRedemptionFailed();
        }

        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "Usuário já possui uma senha definida." }; }

        public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"Usuário já possui a permissão '{role}'." }; }

        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Lockout não está habilitado para este usuário." }; }

        public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"Usuário não tem a permissão '{role}'." }; }
    }
}