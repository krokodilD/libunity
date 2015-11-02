﻿using UnityEngine;
using System;

namespace NetProto
{
    public class NetHandle
    {

        public NetHandle()
        {
        }

        public void Register()
        {
            NetCore.Instance.RegisterHandler(Api.ENetMsgId.get_seed_ack, GetSeedAck);
            NetCore.Instance.RegisterHandler(Api.ENetMsgId.user_login_succeed_ack, UserLoginSucceedAck);
            NetCore.Instance.RegisterHandler(Api.ENetMsgId.user_login_faild_ack, UserLoginFailedAck);
        }

        // 交换dh密钥请求，将发送客户端公钥到服务器
        public void GetSeedReq()
        {
            int sendSeed = NetCore.Instance.GetSendSeed();
            int recvSeed = NetCore.Instance.GetReceiveSeed();
            Proto.seed_info si = new Proto.seed_info();
            si.NetMsgId = (Int16)Api.ENetMsgId.get_seed_req;
            si.client_receive_seed = recvSeed;
            si.client_send_seed = sendSeed;

            NetCore.Instance.Send(Api.ENetMsgId.get_seed_req, si);
        }

        // 服务器返回公钥
        public object GetSeedAck(byte[] data)
        {
            Debug.Log("in GetSeedAck");
            Proto.ByteArray ba = new Proto.ByteArray(data);
            Proto.seed_info si = Proto.seed_info.UnPack(ba);
            ba.Dispose();

            // 启用加密通讯
            NetCore.Instance.Encrypt(si.client_send_seed, si.client_receive_seed);

            // for testing action input
            return si;
        }

        // 游客登陆
        public void UserLoginReq()
        {
            Proto.user_login_info info = new Proto.user_login_info();
            info.NetMsgId = (Int16)Api.ENetMsgId.user_login_req;
            info.login_way = 1;
            info.open_udid = "1021868db6647de4e63d5742baed1e7e44ef265d";
            info.client_certificate = "";
            info.client_version = 123;
            info.user_lang = "zh_CN";
            info.app_id = "foobar1234";
            info.os_version = "Android 6.0 Marshmallow";
            info.device_name = "iPhone 6sp Limited Edition";
            info.device_id = SystemInfo.deviceUniqueIdentifier;
            info.device_id_type = 3;
            info.login_ip = "1.2.3.4";

            NetCore.Instance.Send(Api.ENetMsgId.user_login_req, info);
        }

        // 游客信息
        public object UserLoginSucceedAck(byte[] data)
        {
            Proto.ByteArray ba = new Proto.ByteArray(data);
            Proto.user_snapshot snapshot = Proto.user_snapshot.UnPack(ba);
            ba.Dispose();
            Debug.Log("Welcome userid: " + snapshot.uid);

            return null;
        }

        public object UserLoginFailedAck(byte[] data)
        {
            return null;
        }
    }
}