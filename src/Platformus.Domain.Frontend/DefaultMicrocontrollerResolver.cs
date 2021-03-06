﻿// Copyright © 2017 Dmitry Yegorov. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Platformus.Barebone;
using Platformus.Domain.Data.Abstractions;
using Platformus.Domain.Data.Models;

namespace Platformus.Domain.Frontend
{
  public class DefaultMicrocontrollerResolver : IMicrocontrollerResolver
  {
    public Microcontroller GetMicrocontroller(IRequestHandler requestHandler, string url)
    {
      IEnumerable<Microcontroller> microcontrollers = requestHandler.Storage.GetRepository<IMicrocontrollerRepository>().All();
      
      if (string.IsNullOrEmpty(url))
        return microcontrollers.FirstOrDefault(m => m.UrlTemplate == "{*url}");

      return microcontrollers.FirstOrDefault(m => this.IsMatch(m.UrlTemplate, url));
    }

    public IEnumerable<KeyValuePair<string, string>> GetParameters(string urlTemplate, string url)
    {
      if (string.IsNullOrEmpty(urlTemplate) || string.IsNullOrEmpty(url))
        return new KeyValuePair<string, string>[] { };

      return this.GetNames(urlTemplate).Zip(this.GetValues(url, urlTemplate), (n, v) => new KeyValuePair<string, string>(n, v));
    }

    private bool IsMatch(string urlTemplate, string url)
    {
      return urlTemplate.Count(ch => ch == '/') == url.Count(ch => ch == '/') && Regex.IsMatch(url, this.GetRegexFromUrlTemplate(urlTemplate));
    }

    private IEnumerable<string> GetNames(string urlTemplate)
    {
      return Regex.Matches(urlTemplate, "{.+?}").Cast<Match>().Select(m => m.Value.Replace("{", string.Empty).Replace("}", string.Empty));
    }

    private IEnumerable<string> GetValues(string url, string urlTemplate)
    {
      return Regex.Match(url, this.GetRegexFromUrlTemplate(urlTemplate)).Groups.Cast<Group>().Skip(1).Select(g => g.Value);
    }

    private string GetRegexFromUrlTemplate(string urlTemplate)
    {
      return Regex.Replace(urlTemplate, "{.+?}", "(.+)");
    }
  }
}
