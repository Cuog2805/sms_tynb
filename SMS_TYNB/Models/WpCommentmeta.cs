using System;
using System.Collections.Generic;

namespace SMS_TYNB.Models;

public partial class WpCommentmeta
{
    public ulong MetaId { get; set; }

    public ulong CommentId { get; set; }

    public string? MetaKey { get; set; }

    public string? MetaValue { get; set; }
}
