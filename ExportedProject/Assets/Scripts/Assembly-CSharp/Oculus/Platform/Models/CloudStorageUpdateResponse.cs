using System;

namespace Oculus.Platform.Models
{
	public class CloudStorageUpdateResponse
	{
		public readonly string Bucket;

		public readonly string Key;

		public readonly CloudStorageUpdateStatus Status;

		public readonly string VersionHandle;

		public CloudStorageUpdateResponse(IntPtr o)
		{
			Bucket = CAPI.ovr_CloudStorageUpdateResponse_GetBucket(o);
			Key = CAPI.ovr_CloudStorageUpdateResponse_GetKey(o);
			Status = CAPI.ovr_CloudStorageUpdateResponse_GetStatus(o);
			VersionHandle = CAPI.ovr_CloudStorageUpdateResponse_GetVersionHandle(o);
		}
	}
}
