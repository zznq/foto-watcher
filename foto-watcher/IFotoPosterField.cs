using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace foto_watcher {
    enum FieldType {
        None,
        String,
        File
    }

    interface IFotoPosterField {
        String paramName { get; set; }
        String value { get; set; }
        FieldType type { get; }
    }
}
