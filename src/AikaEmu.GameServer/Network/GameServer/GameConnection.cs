using System;
using AikaEmu.GameServer.Managers;
using AikaEmu.GameServer.Models.Account;
using AikaEmu.GameServer.Models.Units.Character;
using AikaEmu.Shared.Model.Network;
using AikaEmu.Shared.Network;
using NLog;

namespace AikaEmu.GameServer.Network.GameServer
{
    public class GameConnection : BaseConnection
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        public Account Account { get; set; }
        public Character ActiveCharacter => Account.ActiveCharacter;
        public ushort Id { get; set; }

        public GameConnection(Session session) : base(session)
        {
        }

        public void OnDisconnect()
        {
            AccountManager.Instance.RemoveAccount(Id);

            if (Account == null || ActiveCharacter == null)
            {
                _log.Info("[INFO] Account or ActiveCharacter is null, skipping disconnection handling.");
                return;
            }

            if (!ActiveCharacter.IsInternalDisconnect)
            {
                _log.Info("[INFO] Saving ActiveCharacter before disconnect.");
                ActiveCharacter.Save();
            }

            _log.Info("[INFO] Setting ActiveCharacter's friends to offline.");
            ActiveCharacter.Friends.GetOffline();

            _log.Info("[INFO] Despawning ActiveCharacter from the world.");
            WorldManager.Instance.Despawn(ActiveCharacter);
        }

        public void SendPacket(GamePacket packet)
        {
            packet.Connection = this;
            Session.SendPacket(packet.Encode());
            if (packet.Opcode != 0x30bf && packet.Opcode != 0x1001 && packet.Opcode != 0x3049)
                _log.Debug("S->Client: (0x{0:x2}) {1}.", packet.Opcode, (GameOpcode) packet.Opcode);
        }
    }
}