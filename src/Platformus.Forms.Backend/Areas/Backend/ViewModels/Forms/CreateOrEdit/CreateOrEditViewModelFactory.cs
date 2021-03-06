﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Platformus.Barebone;
using Platformus.Forms.Data.Abstractions;
using Platformus.Forms.Data.Models;
using Platformus.Globalization.Backend.ViewModels;
using Platformus.Globalization.Data.Abstractions;

namespace Platformus.Forms.Backend.ViewModels.Forms
{
  public class CreateOrEditViewModelFactory : ViewModelFactoryBase
  {
    public CreateOrEditViewModelFactory(IRequestHandler requestHandler)
      : base(requestHandler)
    {
    }

    public CreateOrEditViewModel Create(int? id)
    {
      if (id == null)
        return new CreateOrEditViewModel()
        {
          NameLocalizations = this.GetLocalizations()
        };

      Form form = this.RequestHandler.Storage.GetRepository<IFormRepository>().WithKey((int)id);

      return new CreateOrEditViewModel()
      {
        Id = form.Id,
        Code = form.Code,
        Email = form.Email,
        NameLocalizations = this.GetLocalizations(this.RequestHandler.Storage.GetRepository<IDictionaryRepository>().WithKey(form.NameId))
      };
    }
  }
}