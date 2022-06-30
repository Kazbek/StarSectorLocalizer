using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaitai;

namespace Localizer.DataExtractors
{
    public class JavaClassExtractor
    {
        private readonly JavaClass _javaClass;
        public JavaClassExtractor(byte[] data)
        {
            _javaClass = new JavaClass(new KaitaiStream(data));
        }

        public JavaClassExtractor(Stream data)
        {
            _javaClass = new JavaClass(new KaitaiStream(data));
        }

        public List<string> GetUtf8Entries()
        {
            //var data = JavaClass.FromFile(@"C:\StarSectorPlayground\ooOO.class");
            //Or parse structure from a byte array:

            //byte[] someArray = new byte[] { ... };
            //var data = new JavaClass(new KaitaiStream(someArray));
            //After that, one can get various attributes from the structure by accessing properties like:

            //var data = new JavaClass(new KaitaiStream(stream));
            List<string> entries = new List<string>();
            foreach (var item in _javaClass.ConstantPool)
            {
                if (item.CpInfo is JavaClass.Utf8CpInfo sub)
                {
                    if (!string.IsNullOrWhiteSpace(sub.Value))
                    {
                        //Console.WriteLine(item.Tag + ": " + sub.Value);
                        entries.Add(sub.Value);
                    }
                    //if(sub.Value == "fleet")
                    //{
                    //    var z = 1;
                    //}
                }
                //else if(item.CpInfo is JavaClass.StringCpInfo st)
                //{
                //    var z = 1;
                //}
                //else
                //{
                //    Console.WriteLine(item.Tag + ": " + item.CpInfo.ToString());
                //}
            }

            foreach(var m in _javaClass.Methods)
            {
                if (entries.Contains(m.NameAsStr))
                {
                    entries.Remove(m.NameAsStr);
                }

                foreach(var a in m.Attributes)
                {
                    if (entries.Contains(a.NameAsStr))
                    {
                        entries.Remove(a.NameAsStr);
                    }
                }
            }


            foreach (var m in _javaClass.Attributes)
            {
                if (entries.Contains(m.NameAsStr))
                {
                    entries.Remove(m.NameAsStr);
                }
            }

            foreach (var m in _javaClass.Fields)
            {
                if (entries.Contains(m.NameAsStr))
                {
                    entries.Remove(m.NameAsStr);
                }
            }

            return entries;
        }

        public List<string> GetProgramUtf8Entries()
        {
            List<string> entries = new List<string>();
            foreach (var m in _javaClass.Methods)
            {
                entries.Add(m.NameAsStr);

                foreach (var a in m.Attributes)
                {
                    entries.Add(a.NameAsStr);
                }
            }


            foreach (var m in _javaClass.Attributes)
            {
                entries.Add(m.NameAsStr);
            }

            foreach (var m in _javaClass.Fields)
            {
                entries.Add(m.NameAsStr);
            }

            return entries;
        }

        private partial class JavaClass : KaitaiStruct
        {
            public static JavaClass FromFile(string fileName)
            {
                return new JavaClass(new KaitaiStream(fileName));
            }

            public JavaClass(KaitaiStream p__io, KaitaiStruct p__parent = null, JavaClass p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root ?? this;
                _read();
            }
            private void _read()
            {
                _magic = m_io.ReadBytes(4);
                if (!((KaitaiStream.ByteArrayCompare(Magic, new byte[] { 202, 254, 186, 190 }) == 0)))
                {
                    throw new ValidationNotEqualError(new byte[] { 202, 254, 186, 190 }, Magic, M_Io, "/seq/0");
                }
                _versionMinor = m_io.ReadU2be();
                _versionMajor = m_io.ReadU2be();
                if (!(VersionMajor >= 43))
                {
                    throw new ValidationLessThanError(43, VersionMajor, M_Io, "/seq/2");
                }
                _constantPoolCount = m_io.ReadU2be();
                _constantPool = new List<ConstantPoolEntry>((int)((ConstantPoolCount - 1)));
                for (var i = 0; i < (ConstantPoolCount - 1); i++)
                {
                    _constantPool.Add(new ConstantPoolEntry((i != 0 ? ConstantPool[(i - 1)].IsTwoEntries : false), m_io, this, m_root));
                }
                _accessFlags = m_io.ReadU2be();
                _thisClass = m_io.ReadU2be();
                _superClass = m_io.ReadU2be();
                _interfacesCount = m_io.ReadU2be();
                _interfaces = new List<ushort>((int)(InterfacesCount));
                for (var i = 0; i < InterfacesCount; i++)
                {
                    _interfaces.Add(m_io.ReadU2be());
                }
                _fieldsCount = m_io.ReadU2be();
                _fields = new List<FieldInfo>((int)(FieldsCount));
                for (var i = 0; i < FieldsCount; i++)
                {
                    _fields.Add(new FieldInfo(m_io, this, m_root));
                }
                _methodsCount = m_io.ReadU2be();
                _methods = new List<MethodInfo>((int)(MethodsCount));
                for (var i = 0; i < MethodsCount; i++)
                {
                    _methods.Add(new MethodInfo(m_io, this, m_root));
                }
                _attributesCount = m_io.ReadU2be();
                _attributes = new List<AttributeInfo>((int)(AttributesCount));
                for (var i = 0; i < AttributesCount; i++)
                {
                    _attributes.Add(new AttributeInfo(m_io, this, m_root));
                }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.5">Source</a>
            /// </remarks>
            public partial class FloatCpInfo : KaitaiStruct
            {
                public static FloatCpInfo FromFile(string fileName)
                {
                    return new FloatCpInfo(new KaitaiStream(fileName));
                }

                public FloatCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _value = m_io.ReadF4be();
                }
                private float _value;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public float Value { get { return _value; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7">Source</a>
            /// </remarks>
            public partial class AttributeInfo : KaitaiStruct
            {
                public static AttributeInfo FromFile(string fileName)
                {
                    return new AttributeInfo(new KaitaiStream(fileName));
                }

                public AttributeInfo(KaitaiStream p__io, KaitaiStruct p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    f_nameAsStr = false;
                    _read();
                }
                private void _read()
                {
                    _nameIndex = m_io.ReadU2be();
                    _attributeLength = m_io.ReadU4be();
                    switch (NameAsStr)
                    {
                        case "SourceFile":
                            {
                                __raw_info = m_io.ReadBytes(AttributeLength);
                                var io___raw_info = new KaitaiStream(__raw_info);
                                _info = new AttrBodySourceFile(io___raw_info, this, m_root);
                                break;
                            }
                        case "LineNumberTable":
                            {
                                __raw_info = m_io.ReadBytes(AttributeLength);
                                var io___raw_info = new KaitaiStream(__raw_info);
                                _info = new AttrBodyLineNumberTable(io___raw_info, this, m_root);
                                break;
                            }
                        case "Exceptions":
                            {
                                __raw_info = m_io.ReadBytes(AttributeLength);
                                var io___raw_info = new KaitaiStream(__raw_info);
                                _info = new AttrBodyExceptions(io___raw_info, this, m_root);
                                break;
                            }
                        case "Code":
                            {
                                __raw_info = m_io.ReadBytes(AttributeLength);
                                var io___raw_info = new KaitaiStream(__raw_info);
                                _info = new AttrBodyCode(io___raw_info, this, m_root);
                                break;
                            }
                        default:
                            {
                                _info = m_io.ReadBytes(AttributeLength);
                                break;
                            }
                    }
                }

                /// <remarks>
                /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.3">Source</a>
                /// </remarks>
                public partial class AttrBodyCode : KaitaiStruct
                {
                    public static AttrBodyCode FromFile(string fileName)
                    {
                        return new AttrBodyCode(new KaitaiStream(fileName));
                    }

                    public AttrBodyCode(KaitaiStream p__io, JavaClass.AttributeInfo p__parent = null, JavaClass p__root = null) : base(p__io)
                    {
                        m_parent = p__parent;
                        m_root = p__root;
                        _read();
                    }
                    private void _read()
                    {
                        _maxStack = m_io.ReadU2be();
                        _maxLocals = m_io.ReadU2be();
                        _codeLength = m_io.ReadU4be();
                        _code = m_io.ReadBytes(CodeLength);
                        _exceptionTableLength = m_io.ReadU2be();
                        _exceptionTable = new List<ExceptionEntry>((int)(ExceptionTableLength));
                        for (var i = 0; i < ExceptionTableLength; i++)
                        {
                            _exceptionTable.Add(new ExceptionEntry(m_io, this, m_root));
                        }
                        _attributesCount = m_io.ReadU2be();
                        _attributes = new List<AttributeInfo>((int)(AttributesCount));
                        for (var i = 0; i < AttributesCount; i++)
                        {
                            _attributes.Add(new AttributeInfo(m_io, this, m_root));
                        }
                    }

                    /// <remarks>
                    /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.3">Source</a>
                    /// </remarks>
                    public partial class ExceptionEntry : KaitaiStruct
                    {
                        public static ExceptionEntry FromFile(string fileName)
                        {
                            return new ExceptionEntry(new KaitaiStream(fileName));
                        }

                        public ExceptionEntry(KaitaiStream p__io, JavaClass.AttributeInfo.AttrBodyCode p__parent = null, JavaClass p__root = null) : base(p__io)
                        {
                            m_parent = p__parent;
                            m_root = p__root;
                            f_catchException = false;
                            _read();
                        }
                        private void _read()
                        {
                            _startPc = m_io.ReadU2be();
                            _endPc = m_io.ReadU2be();
                            _handlerPc = m_io.ReadU2be();
                            _catchType = m_io.ReadU2be();
                        }
                        private bool f_catchException;
                        private ConstantPoolEntry _catchException;
                        public ConstantPoolEntry CatchException
                        {
                            get
                            {
                                if (f_catchException)
                                    return _catchException;
                                if (CatchType != 0)
                                {
                                    _catchException = (ConstantPoolEntry)(M_Root.ConstantPool[(CatchType - 1)]);
                                }
                                f_catchException = true;
                                return _catchException;
                            }
                        }
                        private ushort _startPc;
                        private ushort _endPc;
                        private ushort _handlerPc;
                        private ushort _catchType;
                        private JavaClass m_root;
                        private JavaClass.AttributeInfo.AttrBodyCode m_parent;

                        /// <summary>
                        /// Start of a code region where exception handler is being
                        /// active, index in code array (inclusive)
                        /// </summary>
                        public ushort StartPc { get { return _startPc; } }

                        /// <summary>
                        /// End of a code region where exception handler is being
                        /// active, index in code array (exclusive)
                        /// </summary>
                        public ushort EndPc { get { return _endPc; } }

                        /// <summary>
                        /// Start of exception handler code, index in code array
                        /// </summary>
                        public ushort HandlerPc { get { return _handlerPc; } }

                        /// <summary>
                        /// Exception class that this handler catches, index in constant
                        /// pool, or 0 (catch all exceptions handler, used to implement
                        /// `finally`).
                        /// </summary>
                        public ushort CatchType { get { return _catchType; } }
                        public JavaClass M_Root { get { return m_root; } }
                        public JavaClass.AttributeInfo.AttrBodyCode M_Parent { get { return m_parent; } }
                    }
                    private ushort _maxStack;
                    private ushort _maxLocals;
                    private uint _codeLength;
                    private byte[] _code;
                    private ushort _exceptionTableLength;
                    private List<ExceptionEntry> _exceptionTable;
                    private ushort _attributesCount;
                    private List<AttributeInfo> _attributes;
                    private JavaClass m_root;
                    private JavaClass.AttributeInfo m_parent;
                    public ushort MaxStack { get { return _maxStack; } }
                    public ushort MaxLocals { get { return _maxLocals; } }
                    public uint CodeLength { get { return _codeLength; } }
                    public byte[] Code { get { return _code; } }
                    public ushort ExceptionTableLength { get { return _exceptionTableLength; } }
                    public List<ExceptionEntry> ExceptionTable { get { return _exceptionTable; } }
                    public ushort AttributesCount { get { return _attributesCount; } }
                    public List<AttributeInfo> Attributes { get { return _attributes; } }
                    public JavaClass M_Root { get { return m_root; } }
                    public JavaClass.AttributeInfo M_Parent { get { return m_parent; } }
                }

                /// <remarks>
                /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.5">Source</a>
                /// </remarks>
                public partial class AttrBodyExceptions : KaitaiStruct
                {
                    public static AttrBodyExceptions FromFile(string fileName)
                    {
                        return new AttrBodyExceptions(new KaitaiStream(fileName));
                    }

                    public AttrBodyExceptions(KaitaiStream p__io, JavaClass.AttributeInfo p__parent = null, JavaClass p__root = null) : base(p__io)
                    {
                        m_parent = p__parent;
                        m_root = p__root;
                        _read();
                    }
                    private void _read()
                    {
                        _numberOfExceptions = m_io.ReadU2be();
                        _exceptions = new List<ExceptionTableEntry>((int)(NumberOfExceptions));
                        for (var i = 0; i < NumberOfExceptions; i++)
                        {
                            _exceptions.Add(new ExceptionTableEntry(m_io, this, m_root));
                        }
                    }
                    public partial class ExceptionTableEntry : KaitaiStruct
                    {
                        public static ExceptionTableEntry FromFile(string fileName)
                        {
                            return new ExceptionTableEntry(new KaitaiStream(fileName));
                        }

                        public ExceptionTableEntry(KaitaiStream p__io, JavaClass.AttributeInfo.AttrBodyExceptions p__parent = null, JavaClass p__root = null) : base(p__io)
                        {
                            m_parent = p__parent;
                            m_root = p__root;
                            f_asInfo = false;
                            f_nameAsStr = false;
                            _read();
                        }
                        private void _read()
                        {
                            _index = m_io.ReadU2be();
                        }
                        private bool f_asInfo;
                        private JavaClass.ClassCpInfo _asInfo;
                        public JavaClass.ClassCpInfo AsInfo
                        {
                            get
                            {
                                if (f_asInfo)
                                    return _asInfo;
                                _asInfo = (JavaClass.ClassCpInfo)(((JavaClass.ClassCpInfo)(M_Root.ConstantPool[(Index - 1)].CpInfo)));
                                f_asInfo = true;
                                return _asInfo;
                            }
                        }
                        private bool f_nameAsStr;
                        private string _nameAsStr;
                        public string NameAsStr
                        {
                            get
                            {
                                if (f_nameAsStr)
                                    return _nameAsStr;
                                _nameAsStr = (string)(AsInfo.NameAsStr);
                                f_nameAsStr = true;
                                return _nameAsStr;
                            }
                        }
                        private ushort _index;
                        private JavaClass m_root;
                        private JavaClass.AttributeInfo.AttrBodyExceptions m_parent;
                        public ushort Index { get { return _index; } }
                        public JavaClass M_Root { get { return m_root; } }
                        public JavaClass.AttributeInfo.AttrBodyExceptions M_Parent { get { return m_parent; } }
                    }
                    private ushort _numberOfExceptions;
                    private List<ExceptionTableEntry> _exceptions;
                    private JavaClass m_root;
                    private JavaClass.AttributeInfo m_parent;
                    public ushort NumberOfExceptions { get { return _numberOfExceptions; } }
                    public List<ExceptionTableEntry> Exceptions { get { return _exceptions; } }
                    public JavaClass M_Root { get { return m_root; } }
                    public JavaClass.AttributeInfo M_Parent { get { return m_parent; } }
                }

                /// <remarks>
                /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.10">Source</a>
                /// </remarks>
                public partial class AttrBodySourceFile : KaitaiStruct
                {
                    public static AttrBodySourceFile FromFile(string fileName)
                    {
                        return new AttrBodySourceFile(new KaitaiStream(fileName));
                    }

                    public AttrBodySourceFile(KaitaiStream p__io, JavaClass.AttributeInfo p__parent = null, JavaClass p__root = null) : base(p__io)
                    {
                        m_parent = p__parent;
                        m_root = p__root;
                        f_sourcefileAsStr = false;
                        _read();
                    }
                    private void _read()
                    {
                        _sourcefileIndex = m_io.ReadU2be();
                    }
                    private bool f_sourcefileAsStr;
                    private string _sourcefileAsStr;
                    public string SourcefileAsStr
                    {
                        get
                        {
                            if (f_sourcefileAsStr)
                                return _sourcefileAsStr;
                            _sourcefileAsStr = (string)(((JavaClass.Utf8CpInfo)(M_Root.ConstantPool[(SourcefileIndex - 1)].CpInfo)).Value);
                            f_sourcefileAsStr = true;
                            return _sourcefileAsStr;
                        }
                    }
                    private ushort _sourcefileIndex;
                    private JavaClass m_root;
                    private JavaClass.AttributeInfo m_parent;
                    public ushort SourcefileIndex { get { return _sourcefileIndex; } }
                    public JavaClass M_Root { get { return m_root; } }
                    public JavaClass.AttributeInfo M_Parent { get { return m_parent; } }
                }

                /// <remarks>
                /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.12">Source</a>
                /// </remarks>
                public partial class AttrBodyLineNumberTable : KaitaiStruct
                {
                    public static AttrBodyLineNumberTable FromFile(string fileName)
                    {
                        return new AttrBodyLineNumberTable(new KaitaiStream(fileName));
                    }

                    public AttrBodyLineNumberTable(KaitaiStream p__io, JavaClass.AttributeInfo p__parent = null, JavaClass p__root = null) : base(p__io)
                    {
                        m_parent = p__parent;
                        m_root = p__root;
                        _read();
                    }
                    private void _read()
                    {
                        _lineNumberTableLength = m_io.ReadU2be();
                        _lineNumberTable = new List<LineNumberTableEntry>((int)(LineNumberTableLength));
                        for (var i = 0; i < LineNumberTableLength; i++)
                        {
                            _lineNumberTable.Add(new LineNumberTableEntry(m_io, this, m_root));
                        }
                    }
                    public partial class LineNumberTableEntry : KaitaiStruct
                    {
                        public static LineNumberTableEntry FromFile(string fileName)
                        {
                            return new LineNumberTableEntry(new KaitaiStream(fileName));
                        }

                        public LineNumberTableEntry(KaitaiStream p__io, JavaClass.AttributeInfo.AttrBodyLineNumberTable p__parent = null, JavaClass p__root = null) : base(p__io)
                        {
                            m_parent = p__parent;
                            m_root = p__root;
                            _read();
                        }
                        private void _read()
                        {
                            _startPc = m_io.ReadU2be();
                            _lineNumber = m_io.ReadU2be();
                        }
                        private ushort _startPc;
                        private ushort _lineNumber;
                        private JavaClass m_root;
                        private JavaClass.AttributeInfo.AttrBodyLineNumberTable m_parent;
                        public ushort StartPc { get { return _startPc; } }
                        public ushort LineNumber { get { return _lineNumber; } }
                        public JavaClass M_Root { get { return m_root; } }
                        public JavaClass.AttributeInfo.AttrBodyLineNumberTable M_Parent { get { return m_parent; } }
                    }
                    private ushort _lineNumberTableLength;
                    private List<LineNumberTableEntry> _lineNumberTable;
                    private JavaClass m_root;
                    private JavaClass.AttributeInfo m_parent;
                    public ushort LineNumberTableLength { get { return _lineNumberTableLength; } }
                    public List<LineNumberTableEntry> LineNumberTable { get { return _lineNumberTable; } }
                    public JavaClass M_Root { get { return m_root; } }
                    public JavaClass.AttributeInfo M_Parent { get { return m_parent; } }
                }
                private bool f_nameAsStr;
                private string _nameAsStr;
                public string NameAsStr
                {
                    get
                    {
                        if (f_nameAsStr)
                            return _nameAsStr;
                        _nameAsStr = (string)(((JavaClass.Utf8CpInfo)(M_Root.ConstantPool[(NameIndex - 1)].CpInfo)).Value);
                        f_nameAsStr = true;
                        return _nameAsStr;
                    }
                }
                private ushort _nameIndex;
                private uint _attributeLength;
                private object _info;
                private JavaClass m_root;
                private KaitaiStruct m_parent;
                private byte[] __raw_info;
                public ushort NameIndex { get { return _nameIndex; } }
                public uint AttributeLength { get { return _attributeLength; } }
                public object Info { get { return _info; } }
                public JavaClass M_Root { get { return m_root; } }
                public KaitaiStruct M_Parent { get { return m_parent; } }
                public byte[] M_RawInfo { get { return __raw_info; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.2">Source</a>
            /// </remarks>
            public partial class MethodRefCpInfo : KaitaiStruct
            {
                public static MethodRefCpInfo FromFile(string fileName)
                {
                    return new MethodRefCpInfo(new KaitaiStream(fileName));
                }

                public MethodRefCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    f_classAsInfo = false;
                    f_nameAndTypeAsInfo = false;
                    _read();
                }
                private void _read()
                {
                    _classIndex = m_io.ReadU2be();
                    _nameAndTypeIndex = m_io.ReadU2be();
                }
                private bool f_classAsInfo;
                private JavaClass.ClassCpInfo _classAsInfo;
                public JavaClass.ClassCpInfo ClassAsInfo
                {
                    get
                    {
                        if (f_classAsInfo)
                            return _classAsInfo;
                        _classAsInfo = (JavaClass.ClassCpInfo)(((JavaClass.ClassCpInfo)(M_Root.ConstantPool[(ClassIndex - 1)].CpInfo)));
                        f_classAsInfo = true;
                        return _classAsInfo;
                    }
                }
                private bool f_nameAndTypeAsInfo;
                private JavaClass.NameAndTypeCpInfo _nameAndTypeAsInfo;
                public JavaClass.NameAndTypeCpInfo NameAndTypeAsInfo
                {
                    get
                    {
                        if (f_nameAndTypeAsInfo)
                            return _nameAndTypeAsInfo;
                        _nameAndTypeAsInfo = (JavaClass.NameAndTypeCpInfo)(((JavaClass.NameAndTypeCpInfo)(M_Root.ConstantPool[(NameAndTypeIndex - 1)].CpInfo)));
                        f_nameAndTypeAsInfo = true;
                        return _nameAndTypeAsInfo;
                    }
                }
                private ushort _classIndex;
                private ushort _nameAndTypeIndex;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ushort ClassIndex { get { return _classIndex; } }
                public ushort NameAndTypeIndex { get { return _nameAndTypeIndex; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.5">Source</a>
            /// </remarks>
            public partial class FieldInfo : KaitaiStruct
            {
                public static FieldInfo FromFile(string fileName)
                {
                    return new FieldInfo(new KaitaiStream(fileName));
                }

                public FieldInfo(KaitaiStream p__io, JavaClass p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    f_nameAsStr = false;
                    _read();
                }
                private void _read()
                {
                    _accessFlags = m_io.ReadU2be();
                    _nameIndex = m_io.ReadU2be();
                    _descriptorIndex = m_io.ReadU2be();
                    _attributesCount = m_io.ReadU2be();
                    _attributes = new List<AttributeInfo>((int)(AttributesCount));
                    for (var i = 0; i < AttributesCount; i++)
                    {
                        _attributes.Add(new AttributeInfo(m_io, this, m_root));
                    }
                }
                private bool f_nameAsStr;
                private string _nameAsStr;
                public string NameAsStr
                {
                    get
                    {
                        if (f_nameAsStr)
                            return _nameAsStr;
                        _nameAsStr = (string)(((JavaClass.Utf8CpInfo)(M_Root.ConstantPool[(NameIndex - 1)].CpInfo)).Value);
                        f_nameAsStr = true;
                        return _nameAsStr;
                    }
                }
                private ushort _accessFlags;
                private ushort _nameIndex;
                private ushort _descriptorIndex;
                private ushort _attributesCount;
                private List<AttributeInfo> _attributes;
                private JavaClass m_root;
                private JavaClass m_parent;
                public ushort AccessFlags { get { return _accessFlags; } }
                public ushort NameIndex { get { return _nameIndex; } }
                public ushort DescriptorIndex { get { return _descriptorIndex; } }
                public ushort AttributesCount { get { return _attributesCount; } }
                public List<AttributeInfo> Attributes { get { return _attributes; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.6">Source</a>
            /// </remarks>
            public partial class DoubleCpInfo : KaitaiStruct
            {
                public static DoubleCpInfo FromFile(string fileName)
                {
                    return new DoubleCpInfo(new KaitaiStream(fileName));
                }

                public DoubleCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _value = m_io.ReadF8be();
                }
                private double _value;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public double Value { get { return _value; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.5">Source</a>
            /// </remarks>
            public partial class LongCpInfo : KaitaiStruct
            {
                public static LongCpInfo FromFile(string fileName)
                {
                    return new LongCpInfo(new KaitaiStream(fileName));
                }

                public LongCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _value = m_io.ReadU8be();
                }
                private ulong _value;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ulong Value { get { return _value; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.10">Source</a>
            /// </remarks>
            public partial class InvokeDynamicCpInfo : KaitaiStruct
            {
                public static InvokeDynamicCpInfo FromFile(string fileName)
                {
                    return new InvokeDynamicCpInfo(new KaitaiStream(fileName));
                }

                public InvokeDynamicCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _bootstrapMethodAttrIndex = m_io.ReadU2be();
                    _nameAndTypeIndex = m_io.ReadU2be();
                }
                private ushort _bootstrapMethodAttrIndex;
                private ushort _nameAndTypeIndex;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ushort BootstrapMethodAttrIndex { get { return _bootstrapMethodAttrIndex; } }
                public ushort NameAndTypeIndex { get { return _nameAndTypeIndex; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.8">Source</a>
            /// </remarks>
            public partial class MethodHandleCpInfo : KaitaiStruct
            {
                public static MethodHandleCpInfo FromFile(string fileName)
                {
                    return new MethodHandleCpInfo(new KaitaiStream(fileName));
                }


                public enum ReferenceKindEnum
                {
                    GetField = 1,
                    GetStatic = 2,
                    PutField = 3,
                    PutStatic = 4,
                    InvokeVirtual = 5,
                    InvokeStatic = 6,
                    InvokeSpecial = 7,
                    NewInvokeSpecial = 8,
                    InvokeInterface = 9,
                }
                public MethodHandleCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _referenceKind = ((ReferenceKindEnum)m_io.ReadU1());
                    _referenceIndex = m_io.ReadU2be();
                }
                private ReferenceKindEnum _referenceKind;
                private ushort _referenceIndex;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ReferenceKindEnum ReferenceKind { get { return _referenceKind; } }
                public ushort ReferenceIndex { get { return _referenceIndex; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.6">Source</a>
            /// </remarks>
            public partial class NameAndTypeCpInfo : KaitaiStruct
            {
                public static NameAndTypeCpInfo FromFile(string fileName)
                {
                    return new NameAndTypeCpInfo(new KaitaiStream(fileName));
                }

                public NameAndTypeCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    f_nameAsInfo = false;
                    f_nameAsStr = false;
                    f_descriptorAsInfo = false;
                    f_descriptorAsStr = false;
                    _read();
                }
                private void _read()
                {
                    _nameIndex = m_io.ReadU2be();
                    _descriptorIndex = m_io.ReadU2be();
                }
                private bool f_nameAsInfo;
                private JavaClass.Utf8CpInfo _nameAsInfo;
                public JavaClass.Utf8CpInfo NameAsInfo
                {
                    get
                    {
                        if (f_nameAsInfo)
                            return _nameAsInfo;
                        _nameAsInfo = (JavaClass.Utf8CpInfo)(((JavaClass.Utf8CpInfo)(M_Root.ConstantPool[(NameIndex - 1)].CpInfo)));
                        f_nameAsInfo = true;
                        return _nameAsInfo;
                    }
                }
                private bool f_nameAsStr;
                private string _nameAsStr;
                public string NameAsStr
                {
                    get
                    {
                        if (f_nameAsStr)
                            return _nameAsStr;
                        _nameAsStr = (string)(NameAsInfo.Value);
                        f_nameAsStr = true;
                        return _nameAsStr;
                    }
                }
                private bool f_descriptorAsInfo;
                private JavaClass.Utf8CpInfo _descriptorAsInfo;
                public JavaClass.Utf8CpInfo DescriptorAsInfo
                {
                    get
                    {
                        if (f_descriptorAsInfo)
                            return _descriptorAsInfo;
                        _descriptorAsInfo = (JavaClass.Utf8CpInfo)(((JavaClass.Utf8CpInfo)(M_Root.ConstantPool[(DescriptorIndex - 1)].CpInfo)));
                        f_descriptorAsInfo = true;
                        return _descriptorAsInfo;
                    }
                }
                private bool f_descriptorAsStr;
                private string _descriptorAsStr;
                public string DescriptorAsStr
                {
                    get
                    {
                        if (f_descriptorAsStr)
                            return _descriptorAsStr;
                        _descriptorAsStr = (string)(DescriptorAsInfo.Value);
                        f_descriptorAsStr = true;
                        return _descriptorAsStr;
                    }
                }
                private ushort _nameIndex;
                private ushort _descriptorIndex;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ushort NameIndex { get { return _nameIndex; } }
                public ushort DescriptorIndex { get { return _descriptorIndex; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.7">Source</a>
            /// </remarks>
            public partial class Utf8CpInfo : KaitaiStruct
            {
                public static Utf8CpInfo FromFile(string fileName)
                {
                    return new Utf8CpInfo(new KaitaiStream(fileName));
                }

                public Utf8CpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _strLen = m_io.ReadU2be();
                    _value = System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytes(StrLen));
                }
                private ushort _strLen;
                private string _value;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ushort StrLen { get { return _strLen; } }
                public string Value { get { return _value; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.3">Source</a>
            /// </remarks>
            public partial class StringCpInfo : KaitaiStruct
            {
                public static StringCpInfo FromFile(string fileName)
                {
                    return new StringCpInfo(new KaitaiStream(fileName));
                }

                public StringCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _stringIndex = m_io.ReadU2be();
                }
                private ushort _stringIndex;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ushort StringIndex { get { return _stringIndex; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.9">Source</a>
            /// </remarks>
            public partial class MethodTypeCpInfo : KaitaiStruct
            {
                public static MethodTypeCpInfo FromFile(string fileName)
                {
                    return new MethodTypeCpInfo(new KaitaiStream(fileName));
                }

                public MethodTypeCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _descriptorIndex = m_io.ReadU2be();
                }
                private ushort _descriptorIndex;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ushort DescriptorIndex { get { return _descriptorIndex; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.2">Source</a>
            /// </remarks>
            public partial class InterfaceMethodRefCpInfo : KaitaiStruct
            {
                public static InterfaceMethodRefCpInfo FromFile(string fileName)
                {
                    return new InterfaceMethodRefCpInfo(new KaitaiStream(fileName));
                }

                public InterfaceMethodRefCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    f_classAsInfo = false;
                    f_nameAndTypeAsInfo = false;
                    _read();
                }
                private void _read()
                {
                    _classIndex = m_io.ReadU2be();
                    _nameAndTypeIndex = m_io.ReadU2be();
                }
                private bool f_classAsInfo;
                private JavaClass.ClassCpInfo _classAsInfo;
                public JavaClass.ClassCpInfo ClassAsInfo
                {
                    get
                    {
                        if (f_classAsInfo)
                            return _classAsInfo;
                        _classAsInfo = (JavaClass.ClassCpInfo)(((JavaClass.ClassCpInfo)(M_Root.ConstantPool[(ClassIndex - 1)].CpInfo)));
                        f_classAsInfo = true;
                        return _classAsInfo;
                    }
                }
                private bool f_nameAndTypeAsInfo;
                private JavaClass.NameAndTypeCpInfo _nameAndTypeAsInfo;
                public JavaClass.NameAndTypeCpInfo NameAndTypeAsInfo
                {
                    get
                    {
                        if (f_nameAndTypeAsInfo)
                            return _nameAndTypeAsInfo;
                        _nameAndTypeAsInfo = (JavaClass.NameAndTypeCpInfo)(((JavaClass.NameAndTypeCpInfo)(M_Root.ConstantPool[(NameAndTypeIndex - 1)].CpInfo)));
                        f_nameAndTypeAsInfo = true;
                        return _nameAndTypeAsInfo;
                    }
                }
                private ushort _classIndex;
                private ushort _nameAndTypeIndex;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ushort ClassIndex { get { return _classIndex; } }
                public ushort NameAndTypeIndex { get { return _nameAndTypeIndex; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.1">Source</a>
            /// </remarks>
            public partial class ClassCpInfo : KaitaiStruct
            {
                public static ClassCpInfo FromFile(string fileName)
                {
                    return new ClassCpInfo(new KaitaiStream(fileName));
                }

                public ClassCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    f_nameAsInfo = false;
                    f_nameAsStr = false;
                    _read();
                }
                private void _read()
                {
                    _nameIndex = m_io.ReadU2be();
                }
                private bool f_nameAsInfo;
                private JavaClass.Utf8CpInfo _nameAsInfo;
                public JavaClass.Utf8CpInfo NameAsInfo
                {
                    get
                    {
                        if (f_nameAsInfo)
                            return _nameAsInfo;
                        _nameAsInfo = (JavaClass.Utf8CpInfo)(((JavaClass.Utf8CpInfo)(M_Root.ConstantPool[(NameIndex - 1)].CpInfo)));
                        f_nameAsInfo = true;
                        return _nameAsInfo;
                    }
                }
                private bool f_nameAsStr;
                private string _nameAsStr;
                public string NameAsStr
                {
                    get
                    {
                        if (f_nameAsStr)
                            return _nameAsStr;
                        _nameAsStr = (string)(NameAsInfo.Value);
                        f_nameAsStr = true;
                        return _nameAsStr;
                    }
                }
                private ushort _nameIndex;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ushort NameIndex { get { return _nameIndex; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4">Source</a>
            /// </remarks>
            public partial class ConstantPoolEntry : KaitaiStruct
            {

                public enum TagEnum
                {
                    Utf8 = 1,
                    Integer = 3,
                    Float = 4,
                    Long = 5,
                    Double = 6,
                    ClassType = 7,
                    String = 8,
                    FieldRef = 9,
                    MethodRef = 10,
                    InterfaceMethodRef = 11,
                    NameAndType = 12,
                    MethodHandle = 15,
                    MethodType = 16,
                    InvokeDynamic = 18,
                }
                public ConstantPoolEntry(bool p_isPrevTwoEntries, KaitaiStream p__io, JavaClass p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _isPrevTwoEntries = p_isPrevTwoEntries;
                    f_isTwoEntries = false;
                    _read();
                }
                private void _read()
                {
                    if (!(IsPrevTwoEntries))
                    {
                        _tag = ((TagEnum)m_io.ReadU1());
                    }
                    if (!(IsPrevTwoEntries))
                    {
                        switch (Tag)
                        {
                            case TagEnum.InterfaceMethodRef:
                                {
                                    _cpInfo = new InterfaceMethodRefCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.ClassType:
                                {
                                    _cpInfo = new ClassCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.Utf8:
                                {
                                    _cpInfo = new Utf8CpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.MethodType:
                                {
                                    _cpInfo = new MethodTypeCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.Integer:
                                {
                                    _cpInfo = new IntegerCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.String:
                                {
                                    _cpInfo = new StringCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.Float:
                                {
                                    _cpInfo = new FloatCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.Long:
                                {
                                    _cpInfo = new LongCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.MethodRef:
                                {
                                    _cpInfo = new MethodRefCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.Double:
                                {
                                    _cpInfo = new DoubleCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.InvokeDynamic:
                                {
                                    _cpInfo = new InvokeDynamicCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.FieldRef:
                                {
                                    _cpInfo = new FieldRefCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.MethodHandle:
                                {
                                    _cpInfo = new MethodHandleCpInfo(m_io, this, m_root);
                                    break;
                                }
                            case TagEnum.NameAndType:
                                {
                                    _cpInfo = new NameAndTypeCpInfo(m_io, this, m_root);
                                    break;
                                }
                        }
                    }
                }
                private bool f_isTwoEntries;
                private bool _isTwoEntries;
                public bool IsTwoEntries
                {
                    get
                    {
                        if (f_isTwoEntries)
                            return _isTwoEntries;
                        _isTwoEntries = (bool)((IsPrevTwoEntries ? false : ((Tag == TagEnum.Long) || (Tag == TagEnum.Double))));
                        f_isTwoEntries = true;
                        return _isTwoEntries;
                    }
                }
                private TagEnum _tag;
                private KaitaiStruct _cpInfo;
                private bool _isPrevTwoEntries;
                private JavaClass m_root;
                private JavaClass m_parent;
                public TagEnum Tag { get { return _tag; } }
                public KaitaiStruct CpInfo { get { return _cpInfo; } }
                public bool IsPrevTwoEntries { get { return _isPrevTwoEntries; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.6">Source</a>
            /// </remarks>
            public partial class MethodInfo : KaitaiStruct
            {
                public static MethodInfo FromFile(string fileName)
                {
                    return new MethodInfo(new KaitaiStream(fileName));
                }

                public MethodInfo(KaitaiStream p__io, JavaClass p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    f_nameAsStr = false;
                    _read();
                }
                private void _read()
                {
                    _accessFlags = m_io.ReadU2be();
                    _nameIndex = m_io.ReadU2be();
                    _descriptorIndex = m_io.ReadU2be();
                    _attributesCount = m_io.ReadU2be();
                    _attributes = new List<AttributeInfo>((int)(AttributesCount));
                    for (var i = 0; i < AttributesCount; i++)
                    {
                        _attributes.Add(new AttributeInfo(m_io, this, m_root));
                    }
                }
                private bool f_nameAsStr;
                private string _nameAsStr;
                public string NameAsStr
                {
                    get
                    {
                        if (f_nameAsStr)
                            return _nameAsStr;
                        _nameAsStr = (string)(((JavaClass.Utf8CpInfo)(M_Root.ConstantPool[(NameIndex - 1)].CpInfo)).Value);
                        f_nameAsStr = true;
                        return _nameAsStr;
                    }
                }
                private ushort _accessFlags;
                private ushort _nameIndex;
                private ushort _descriptorIndex;
                private ushort _attributesCount;
                private List<AttributeInfo> _attributes;
                private JavaClass m_root;
                private JavaClass m_parent;
                public ushort AccessFlags { get { return _accessFlags; } }
                public ushort NameIndex { get { return _nameIndex; } }
                public ushort DescriptorIndex { get { return _descriptorIndex; } }
                public ushort AttributesCount { get { return _attributesCount; } }
                public List<AttributeInfo> Attributes { get { return _attributes; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.4">Source</a>
            /// </remarks>
            public partial class IntegerCpInfo : KaitaiStruct
            {
                public static IntegerCpInfo FromFile(string fileName)
                {
                    return new IntegerCpInfo(new KaitaiStream(fileName));
                }

                public IntegerCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _value = m_io.ReadU4be();
                }
                private uint _value;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public uint Value { get { return _value; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }

            /// <remarks>
            /// Reference: <a href="https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.2">Source</a>
            /// </remarks>
            public partial class FieldRefCpInfo : KaitaiStruct
            {
                public static FieldRefCpInfo FromFile(string fileName)
                {
                    return new FieldRefCpInfo(new KaitaiStream(fileName));
                }

                public FieldRefCpInfo(KaitaiStream p__io, JavaClass.ConstantPoolEntry p__parent = null, JavaClass p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    f_classAsInfo = false;
                    f_nameAndTypeAsInfo = false;
                    _read();
                }
                private void _read()
                {
                    _classIndex = m_io.ReadU2be();
                    _nameAndTypeIndex = m_io.ReadU2be();
                }
                private bool f_classAsInfo;
                private JavaClass.ClassCpInfo _classAsInfo;
                public JavaClass.ClassCpInfo ClassAsInfo
                {
                    get
                    {
                        if (f_classAsInfo)
                            return _classAsInfo;
                        _classAsInfo = (JavaClass.ClassCpInfo)(((JavaClass.ClassCpInfo)(M_Root.ConstantPool[(ClassIndex - 1)].CpInfo)));
                        f_classAsInfo = true;
                        return _classAsInfo;
                    }
                }
                private bool f_nameAndTypeAsInfo;
                private JavaClass.NameAndTypeCpInfo _nameAndTypeAsInfo;
                public JavaClass.NameAndTypeCpInfo NameAndTypeAsInfo
                {
                    get
                    {
                        if (f_nameAndTypeAsInfo)
                            return _nameAndTypeAsInfo;
                        _nameAndTypeAsInfo = (JavaClass.NameAndTypeCpInfo)(((JavaClass.NameAndTypeCpInfo)(M_Root.ConstantPool[(NameAndTypeIndex - 1)].CpInfo)));
                        f_nameAndTypeAsInfo = true;
                        return _nameAndTypeAsInfo;
                    }
                }
                private ushort _classIndex;
                private ushort _nameAndTypeIndex;
                private JavaClass m_root;
                private JavaClass.ConstantPoolEntry m_parent;
                public ushort ClassIndex { get { return _classIndex; } }
                public ushort NameAndTypeIndex { get { return _nameAndTypeIndex; } }
                public JavaClass M_Root { get { return m_root; } }
                public JavaClass.ConstantPoolEntry M_Parent { get { return m_parent; } }
            }
            private byte[] _magic;
            private ushort _versionMinor;
            private ushort _versionMajor;
            private ushort _constantPoolCount;
            private List<ConstantPoolEntry> _constantPool;
            private ushort _accessFlags;
            private ushort _thisClass;
            private ushort _superClass;
            private ushort _interfacesCount;
            private List<ushort> _interfaces;
            private ushort _fieldsCount;
            private List<FieldInfo> _fields;
            private ushort _methodsCount;
            private List<MethodInfo> _methods;
            private ushort _attributesCount;
            private List<AttributeInfo> _attributes;
            private JavaClass m_root;
            private KaitaiStruct m_parent;
            public byte[] Magic { get { return _magic; } }
            public ushort VersionMinor { get { return _versionMinor; } }
            public ushort VersionMajor { get { return _versionMajor; } }
            public ushort ConstantPoolCount { get { return _constantPoolCount; } }
            public List<ConstantPoolEntry> ConstantPool { get { return _constantPool; } }
            public ushort AccessFlags { get { return _accessFlags; } }
            public ushort ThisClass { get { return _thisClass; } }
            public ushort SuperClass { get { return _superClass; } }
            public ushort InterfacesCount { get { return _interfacesCount; } }
            public List<ushort> Interfaces { get { return _interfaces; } }
            public ushort FieldsCount { get { return _fieldsCount; } }
            public List<FieldInfo> Fields { get { return _fields; } }
            public ushort MethodsCount { get { return _methodsCount; } }
            public List<MethodInfo> Methods { get { return _methods; } }
            public ushort AttributesCount { get { return _attributesCount; } }
            public List<AttributeInfo> Attributes { get { return _attributes; } }
            public JavaClass M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
    }
}
