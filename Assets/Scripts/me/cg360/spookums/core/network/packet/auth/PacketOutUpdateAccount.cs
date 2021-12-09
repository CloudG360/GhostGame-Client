using me.cg360.spookums.utility;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace me.cg360.spookums.core.network.packet.auth
{
    // This packet contains a lot of optional peices of data. They are
    // ordered based on their apperence in the flags.
    public class PacketOutUpdateAccount : NetworkPacket
    {

        protected static readonly int EXISTING_PASSWORD_ID = 1;
        protected static readonly int NEW_PASSWORD_ID = 2;
        protected static readonly int EXISTING_USERNAME_ID = 3;
        protected static readonly int NEW_USERNAME_ID = 4;

        public bool IsCreatingNewAccount { get; set; } = false;

        public string ExistingPassword { get; set; } = null;
        public string NewPassword { get; set; } = null;

        public string ExistingUsername { get; set; } = null;
        public string NewUsername { get; set; } = null;


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
            ushort size = 1;
            
            MicroBoolean flags = new MicroBoolean();
            Body.PutUnsignedByte(0x00); // Temporary
            


            flags.SetValue(0, IsCreatingNewAccount);
            
            size += AddOptionalString(flags, 1, ExistingPassword);
            size += AddOptionalString(flags, 2, NewPassword);
            size += AddOptionalString(flags, 3, ExistingUsername);
            size += AddOptionalString(flags, 4, NewUsername);
            
            Body.Reset();
            Body.PutUnsignedByte(flags.GetStorageByte());
            
            return size;
        }

        protected override void DecodeBody(ushort inboundSize)
        {
            Body.Reset();
        }


        protected ushort AddOptionalString(MicroBoolean flags, int index, string value)
        {
            flags.SetValue(index, value != null);
            return Body.PutSmallUTF8String(value);
        }


        public bool IsValid()
        {
            return GetMissingFields().IsEmpty();
        }


        public MicroBoolean GetMissingFields()
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
    }
}