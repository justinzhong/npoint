using System;
using System.Collections.Specialized;

namespace NPoint.Transport
{
    public interface IUriQueryAppender
    {
        Uri AppendQuery(Uri url, NameValueCollection nameValues);
        Uri AppendQuery(Uri url, string name, string value);
    }
}