using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HipChat.Entities
{
   
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.XmlTypeAttribute( AnonymousType = true )]
    [System.Xml.Serialization.XmlRootAttribute( Namespace = "", IsNullable = false )]
    public partial class room
    {

        private byte room_idField;

        private string nameField;

        private string topicField;

        private uint last_activeField;

        private uint createdField;

        private byte is_archivedField;

        private byte is_privateField;

        private byte owner_user_idField;

        private byte[] member_user_idsField;

        private roomParticipant[] participantsField;

        private object guest_access_urlField;

        private string xmpp_jidField;

        /// <remarks/>
        public byte room_id
        {
            get
            {
                return this.room_idField;
            }
            set
            {
                this.room_idField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string topic
        {
            get
            {
                return this.topicField;
            }
            set
            {
                this.topicField = value;
            }
        }

        /// <remarks/>
        public uint last_active
        {
            get
            {
                return this.last_activeField;
            }
            set
            {
                this.last_activeField = value;
            }
        }

        /// <remarks/>
        public uint created
        {
            get
            {
                return this.createdField;
            }
            set
            {
                this.createdField = value;
            }
        }

        /// <remarks/>
        public byte is_archived
        {
            get
            {
                return this.is_archivedField;
            }
            set
            {
                this.is_archivedField = value;
            }
        }

        /// <remarks/>
        public byte is_private
        {
            get
            {
                return this.is_privateField;
            }
            set
            {
                this.is_privateField = value;
            }
        }

        /// <remarks/>
        public byte owner_user_id
        {
            get
            {
                return this.owner_user_idField;
            }
            set
            {
                this.owner_user_idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute()]
        [System.Xml.Serialization.XmlArrayItemAttribute( "member_user_id", IsNullable = false )]
        public byte[] member_user_ids
        {
            get
            {
                return this.member_user_idsField;
            }
            set
            {
                this.member_user_idsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute( "participant", IsNullable = false )]
        public roomParticipant[] participants
        {
            get
            {
                return this.participantsField;
            }
            set
            {
                this.participantsField = value;
            }
        }

        /// <remarks/>
        public object guest_access_url
        {
            get
            {
                return this.guest_access_urlField;
            }
            set
            {
                this.guest_access_urlField = value;
            }
        }

        /// <remarks/>
        public string xmpp_jid
        {
            get
            {
                return this.xmpp_jidField;
            }
            set
            {
                this.xmpp_jidField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.XmlTypeAttribute( AnonymousType = true )]
    public partial class roomParticipant
    {

        private byte user_idField;

        private string nameField;

        /// <remarks/>
        public byte user_id
        {
            get
            {
                return this.user_idField;
            }
            set
            {
                this.user_idField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }


}
