# $Id: 200_reg_good_enocredentiall.py 2110 2008-07-07 20:14:41Z bennylp $
import inc_sip as sip
import inc_sdp as sdp

pjsua = "--null-audio --id=sip:CLIENT --registrar sip:127.0.0.1:$PORT"

req1 = sip.RecvfromTransaction("", 401,
				include=["REGISTER sip"], 
				exclude=["Authorization"],
				resp_hdr=["WWW-Authenticate: Digest realm=\"python\", nonce=\"1234\""],
				expect="PJSIP_ENOCREDENTIAL"
			  )

recvfrom_cfg = sip.RecvfromCfg("Failed registration test",
			       pjsua, [req1])
