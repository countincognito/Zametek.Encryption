﻿using FluentValidation;

namespace Zametek.Access.Encryption
{
    public class RemoveSymmetricKeyRequestValidator
        : AbstractValidator<RemoveSymmetricKeyRequest>
    {
        private static readonly RemoveSymmetricKeyRequestValidator s_Instance = new();

        protected RemoveSymmetricKeyRequestValidator()
        {
            RuleFor(request => request).NotNull();
            RuleFor(request => request.SymmetricKeyId).NotEmpty();
            RuleFor(request => request.AsymmetricKeyId).NotEmpty();
        }

        public static async Task ValidateAndThrowAsync(
            RemoveSymmetricKeyRequest request,
            CancellationToken ct)
        {
            await s_Instance
                .ValidateAndThrowAsync(request, cancellationToken: ct)
                .ConfigureAwait(false);
        }
    }
}
