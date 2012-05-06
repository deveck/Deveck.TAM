# $Id: 150_srtp_1_1.py 2025 2008-06-15 19:43:43Z bennylp $
#
from inc_cfg import *

test_param = TestParam(
		"Callee=optional SRTP, caller=optional SRTP",
		[
			InstanceParam("callee", "--null-audio --use-srtp=1 --srtp-secure=0 --max-calls=1"),
			InstanceParam("caller", "--null-audio --use-srtp=1 --srtp-secure=0 --max-calls=1")
		]
		)
