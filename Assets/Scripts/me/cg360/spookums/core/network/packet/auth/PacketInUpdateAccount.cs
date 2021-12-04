using me.cg360.spookums.utility;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace me.cg360.spookums.core.network.packet.auth
{
    // This packet contains a lot of optional peices of data. They are
    // ordered based on their apperence in the flags.
    public class PacketInUpdateAccount : NetworkPacket
    {

        protected static readonly int EXISTING_PASSWORD_ID = 1;
        protected static readonly int NEW_PASSWORD_ID = 2;
        protected static readonly int EXISTING_USERNAME_ID = 3;
        protected static readonly int NEW_USERNAME_ID = 4;

        public bool IsCreatingNewAccount { get; set; } = false;

        protected string ExistingPassword { get; set; } = null;
        protected string NewPassword { get; set; } = null;

        protected string ExistingUsername { get; set; } = null;
        protected string NewUsername { get; set; } = null;


        protected override byte GetPacketTypeID()
        {
            return VanillaProtocol.PACKET_UPDATE_ACCOUNT;
        }


        protected override ushort EncodeBody()
        {
            // -- Flags:
            //createNewAccount: Is this packet meant to create a new account?
            //existingPassword: Depending on the data, is the original password required for further verification?
            //newPassword: Is a new password being set? (Requires original password if updating)
            //existingUsername: Used to indicate which account is being updated.
            //newUsername: Used to set a new username for an update/new account.
            return 0;
        }

        protected override void DecodeBody(ushort inboundSize)
        {
            Body.Reset();
        }


        protected void ifStringFlagTrue(MicroBoolean flags, int index, ThreadStart doThis)
        {
            if (flags.GetValue(index) && this.Body.CanReadBytesAhead(1))
            {
                doThis.Invoke();
            }
        }


        public bool isValid()
        {
            return getMissingFields().IsEmpty();
        }


        public MicroBoolean getMissingFields()
        {
            if (this.IsCreatingNewAccount)
            {
                MicroBoolean missingFields = MicroBoolean.Empty();
                missingFields.SetValue(NEW_USERNAME_ID, Check.IsNull(this.NewUsername));
                missingFields.SetValue(NEW_PASSWORD_ID, Check.IsNull(this.NewPassword));

                return missingFields;

            }
            else
            {
                return MicroBoolean.Empty()
                        .SetValue(EXISTING_USERNAME_ID, Check.IsNull(this.ExistingUsername)); // Some changes don't require the password.
            }
        }
        


        protected static bool IsFilledString(string str)
        {
            return (str != null) && (str.Length > 0);
        }
    }
}