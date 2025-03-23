using System;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.GameServer.Network.Packets.Client;
using AikaEmu.Shared.Network;
using AikaEmu.Shared.Packets;
using NLog;

namespace AikaEmu.GameServer.Network.Packets.Client
{
    public class RequestCheckLogin : GamePacket
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private uint _accountId;
        private string _username;
        private string _password;

        // Construtor sem parâmetros necessário para instância dinâmica
        public RequestCheckLogin() { }

        // Construtor original
        public RequestCheckLogin(uint accountId, string username, string password)
        {
            Initialize(accountId, username, password);
        }

        // Método para inicializar os campos
        public void Initialize(uint accountId, string username, string password)
        {
            Opcode = (ushort)ClientOpcode.RequestCheckLogin;
            _accountId = accountId;
            _username = username;
            _password = password;

            // Log no momento da inicialização
            _log.Error($"[DEBUG] RequestCheckLogin inicializado - AccountId: {_accountId}, Username: {_username}, Password: {_password}");
        }

        public override PacketStream Write(PacketStream stream)
        {
            // Log antes de escrever os dados no stream
            _log.Error($"[DEBUG] Escrevendo dados no pacote RequestCheckLogin - AccountId: {_accountId}, Username: {_username}, Password: {_password}");
            
            stream.Write(_accountId);
            stream.Write(_username, 32);
            stream.Write(_password, 32);

            return stream;
        }
    }
}
