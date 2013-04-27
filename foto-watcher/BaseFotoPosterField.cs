using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace foto_watcher {
    class BaseFotoPosterField : IFotoPosterField {
        public BaseFotoPosterField(FieldType type) {
            this.type = type;
        }

        public string paramName {
            get;
            set;
        }

        public string value {
            get;
            set;
        }

        public FieldType type {
            get;
            protected set;
        }
    }
}
