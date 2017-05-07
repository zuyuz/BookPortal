(function(window, undefined) {

 var AU = (window.Aurigma || (window.Aurigma = { __namespace: true })) &&
    (window.Aurigma.ImageUploaderFlash || (window.Aurigma.ImageUploaderFlash = { __namespace: true }));

AU.language || (AU.language = { __namespace: true });

var $au = AU;


function prop(name, value, setter) {
    if (setter) {
        // set passed value
        this['_' + name] = value;
    } else {
        // return current value
        return this['_' + name];
    }
}

AU.amazonS3Extender = function (uploader) {
    ///	<summary>
    ///		Create Amazon S3 extenser
    ///	</summary>
    ///	<param name="uploader" type="$au.uploader">
    ///		Uploader
    ///	</param>
    ///	<returns type="$au.amazonS3Extender" />
    if (!(this instanceof AU.amazonS3Extender)) {
        return new AU.amazonS3Extender(uploader);
    }

    if (uploader.state() == 1) {
        $au.debug().showError('AmazonS3Extender should be created before uploader initialization.');
    }
    uploader.amazonS3Extender(this);
    this._uploader = uploader;
    this._converters = new AU.amazonS3Extender.converters();
    this._converterIndex = -1;
    var self = this;

    uploader.events().beforeSendRequest().add(function () { return self._onBeforeSendRequest.apply(self, arguments); });
    uploader.events().initComplete().add(function () { return self._onInitComplete.apply(self, arguments); });
    uploader.events().beforeUpload().add(function () { return self._onBeforeUpload.apply(self, arguments); });
};

AU.amazonS3Extender.prototype = {
    __class: true,

    // TODO: Make sure if all field names are presented here and the names are corrected.
    predefinedFields: {
        description: "Description_[itemIndex]",
        width: "Width_[itemIndex]",
        height: "Height_[itemIndex]",
        angle: "Angle_[itemIndex]",
        sourceName: "SourceName_[itemIndex]",
        horizontalResolution: "HorizontalResolution_[itemIndex]",
        verticalResolution: "VerticalResolution_[itemIndex]",
        sourceFileSize: "SourceSize_[itemIndex]",
        sourceCreatedDateTime: "SourceCreatedDateTime_[itemIndex]",
        sourceLastModifiedDateTime: "SourceLastModifiedDateTime_[itemIndex]",
        sourceCreatedDateTimeLocal: "SourceCreatedDateTimeLocal_[itemIndex]",
        sourceLastModifiedDateTimeLocal: "SourceLastModifiedDateTimeLocal_[itemIndex]",
        thumbnailSucceeded: "Thumbnail[converterIndex]Succeeded_[itemIndex]",
        fileMode: "File[converterIndex]Mode_[itemIndex]",
        fileSize: "File[converterIndex]Size_[itemIndex]",
        fileWidth: "File[converterIndex]Width_[itemIndex]",
        fileHeight: "File[converterIndex]Height_[itemIndex]",
        fileName: "File[converterIndex]Name_[itemIndex]",
        packageFileCount: "PackageFileCount",
        packageIndex: "PackageIndex",
        packageCount: "PackageCount",
        packageGuid: "PackageGuid",
        cropBounds: "CropBounds_[itemIndex]"
    },

    _prop: prop,

    bucket: function (value) {
        ///	<summary>
        ///		Get or set bucket
        ///	</summary>
        ///	<param name="value" type="String" />
        ///	<returns type="String" />
        return this._prop('bucket', value, arguments.length);
    },
    accessKeyId: function (value) {
        ///	<summary>
        ///		Get or set AWS access key
        ///	</summary>
        ///	<param name="value" type="String" />
        ///	<returns type="String" />
        return this._prop('accessKeyId', value, arguments.length);
    },
    region: function (value) {
        ///	<summary>
        ///		Get or set AWS region (e.g. us-east-1).
        ///	</summary>
        ///	<param name="value" type="String" />
        ///	<returns type="String" />
        return this._prop('region', value, arguments.length);
    },
    bucketHostName: function (value) {
        ///	<summary>
        ///		Get or set bucket host name. Bucket host name. "s3.amazonaws.com" will be used if not specified.
        ///	</summary>
        ///	<param name="value" type="String" />
        ///	<returns type="String" />
        return this._prop('bucketHostName', value, arguments.length);
    },
    checkIntegrity: function (value) {
        ///	<summary>
        ///		Specify whether to check data integrity while uploading files.
        ///	</summary>
        ///	<param name="value" type="Boolean" />
        ///	<returns type="Boolean" />
        return this._prop('checkIntegrity', value, arguments.length);
    },
    converters: function (value) {
        ///	<summary>
        ///		Get or set settings for file for every converter
        ///	</summary>
        ///	<param name="value" type="Array">
        ///		Array of settings for converters files
        ///	</param>
        ///	<returns type="$au.amazonS3Extender.converters" />
        if (arguments.length == 0) {
            return this._converters;
        } else {
            this._converters.set(value);
        }
    },
    _onBeforeSendRequest: function (index, fileName, packageGuid, fileGuid) {
        var u = this._uploader, converterIndex = (++this._converterIndex % this._converterCount), m = u.metadata(),
                amzMetaPrefix = 'x-amz-meta-';

        var file = u.files().get(fileGuid);

        // disable standard fiels
        m.enableAllStandardFields(false);
        m.resetCustomFields();
        // http://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-authentication-HTTPPOST.html
	// now we should send credential field:
	// <your-access-key-id>/<date>/<aws-region>/<aws-service>/aws4_request
	var now_string = this._getNowString();
	m.addCustomField("x-amz-credential", this.accessKeyId() + "/" + now_string + "/" + this.region() + "/s3/aws4_request");
	// and algorithm field: 
	m.addCustomField("x-amz-algorithm", "AWS4-HMAC-SHA256");
	// and date field with should be in ISO format (20160101T000000Z): 
	m.addCustomField("x-amz-date", now_string + "T000000Z");
        m.addCustomField("success_action_status", "200");
        // rename and enable file field
        m.enableStandardField("File[converterIndex]_[itemIndex]", true);
        m.renameStandardField("File[converterIndex]_[itemIndex]", "file");
                                     
        if (this.checkIntegrity()) {
            m.enableStandardField("File[converterIndex]HashCodeMD5_[itemIndex]", true);
            m.renameStandardField("File[converterIndex]HashCodeMD5_[itemIndex]", "Content-MD5");
        }

        // clear custom fields from previous file
        var prevMeta = this.converters().get((converterIndex + this._converterCount - 1) % this._converterCount).meta();
        if (prevMeta && prevMeta.length > 0) {
            for (var i = 0, imax = prevMeta.length; i < imax; i++) {
                var mm = prevMeta[i];
                if (mm.name && mm.value != null) {
                    m.removeCustomField(amzMetaPrefix + mm.name);
                }
            }
        }
        
        // add files fields
        var f = this.converters().get(converterIndex);
        if (f) {
            
            var key =  f.key() ? f.key() : "";
            var reFileName = /\$\{fileName\}/ig;
            var reGuid = /\$\{guid\}/ig;
            var rePath = /\$\{path\}/ig;

            if(rePath.test(key)){
               key = key.replace(rePath, u.files().get(0).relativePath()).replace(/[\\]+|[/]+/g,"/");  
            }

            if(reFileName.test(key)){
               key = key.replace(reFileName, fileName)  
            }

            if(reGuid.test(key)){
               var pointIndex = fileName.lastIndexOf(".");
               var ext = pointIndex > -1 ? fileName.substr(pointIndex, fileName.length) : "";
               key = key.replace(reGuid, packageGuid + ext)  
            }
 
            m.addCustomField('acl', f.acl());
            m.addCustomField('key', key) ;
            m.addCustomField('policy', f.policy());
            m.addCustomField('x-amz-signature', f.signature());

	    
            var contentType = f.contentType();

            if (contentType != null && contentType != '') {
                m.addCustomField('Content-Type', contentType);
            }

            if(f.cacheControl()){
                m.addCustomField('Cache-Control', f.cacheControl());
            }

            if(f.expires()){
	        m.addCustomField('Expires', f.expires());
            }


            if(f.storageClass()){
	    	m.addCustomField('x-amz-storage-class', f.storageClass());
            }


            // Add custom fields
            var meta = f.meta();
            if (meta && meta.length > 0) {
                for (var i = 0, imax = meta.length; i < imax; i++) {
                    var mm = meta[i], name = mm.name;
                    if (name) {
                        var value = meta[i].value, field = meta[i].field;
                        if (field != null) {

                            if(/exif/i.test(field)){
                                var val = file.exif()[field.replace(/exif/i, "")];
                                m.addCustomField(amzMetaPrefix + name, typeof val == "undefined" ? "" : val);
                            }else if(/iptc/i.test(field)){
                                var val = file.iptc()[field];
                                m.addCustomField(amzMetaPrefix + name, typeof val == "undefined" ? "" : val);
			    }else{
                                // Add standard field
                                m.enableStandardField(field, true);
                                m.renameStandardField(field, amzMetaPrefix + name);
                            }
                            
                        } else if (value != null) {
                            // Add custom field
                            m.addCustomField(amzMetaPrefix + name, value);
                        }
                    }
                }
            }
        }

        this._converterIndex = converterIndex;

    },
    _getNowString : function(){ 
	var d = new Date();
	var result = d.getFullYear().toString();
	var m = (d.getMonth() + 1).toString();
	result += m.length == 1 ? "0" + m : m;
	var day = d.getDate().toString();
	result += day.length == 1 ? "0" + day : day;
	return result; 
    },
    _onInitComplete: function () {
        var u = this._uploader;

        // Set action to Amazon S3
        var hostName = this.bucketHostName() ? this.bucketHostName() : this.bucket() + ".s3.amazonaws.com";
        var us = u.uploadSettings();
        us.actionUrl(window.location.protocol + "//" + hostName + "/");

        // Upload one file per request.
        us.uploadConverterOutputSeparately(true);

        // To prepare and upload files simultaneously.
        us.filesPerPackage(1);

        // BUG 0014526: Disable sending HEAD request
        if (u.type() == 'java') {
            us.enableHeadRequest(false);
        }

        // Chunk upload on Amazon S3 is unsupported.
        us.chunkSize(0);


	if (!this.region()) {
	        $au.debug().showError("Now you must specify the region field along with other AmazonS3Extender settings.");
	}
	u.metadata().uploadRequestHeaders({});

    },
    _onBeforeUpload: function () {
        this._converterCount = this._uploader.converters().count();
        this._converterIndex = -1;

        var converters = this._uploader.converters(),
                hash = this.checkIntegrity() ? 'MD5BASE64' : '';
        for (var i = 0, imax = converters.count(); i < imax; i++) {
            converters.get(i).hash(hash);
        }
    }
};

AU.amazonS3Extender.prototype.constructor = AU.amazonS3Extender;

AU.amazonS3Extender.converters = function () {
    ///	<summary>
    ///		Get or set settings for file for every converter
    ///	</summary>
    ///	<param name="value" type="Array">
    ///		Array of settings for files
    ///	</param>
    ///	<returns type="$au.amazonS3Extender.converters" />
    if (!(this instanceof AU.amazonS3Extender.converters)) {
        return new AU.amazonS3Extender.converters();
    }

    this._list = {};
};

AU.amazonS3Extender.converters.prototype = {
    __class: true,

    clear: function () {
        ///	<summary>
        ///		Clear settings for all converters
        ///	</summary>
        this._list = {};
    },
    count: function () {
        ///	<summary>
        ///		
        ///	</summary>
        return Object.keys(this._list).length;
    },
    get: function (index) {
        ///	<summary>
        ///		Get settings for particular converter
        ///	</summary>
        ///	<param name="index" type="Number">
        ///		Index of converter settings
        ///	</param>
        ///	<returns type="$au.amazonS3Extender.fileSettings" />
        if (!this._list[index]) {
            this._list[index] = new AU.amazonS3Extender.fileSettings();
        }
        return this._list[index];
    },
    remove: function (index) {
        ///	<summary>
        ///		Remove settings for particular converter
        ///	</summary>
        ///	<param name="index" type="Number">
        ///		Index of converter
        ///	</param>
        this._list.splice(index, 1);
    },
    set: function (obj) {
        for (var i = 0, imax = obj.length; i < imax; i++) {
            var fs = new AU.amazonS3Extender.fileSettings(), opt = obj[i];
            this._list[i] = fs;
            for (var name in opt) {
                if (!opt.hasOwnProperty || opt.hasOwnProperty(name)) {
                    if (typeof fs[name] === 'function') {
                        fs[name](opt[name]);
                    } else {
                        showWarning('$au.amazonS3Extender.fileSettings has not "' + name + '" property.');
                    }
                }
            }
        }
    }
};

AU.amazonS3Extender.converters.prototype.constructor = AU.amazonS3Extender.converters;

AU.amazonS3Extender.fileSettings = function () {
    if (!(this instanceof AU.amazonS3Extender.fileSettings)) {
        return new AU.amazonS3Extender.fileSettings();
    }
    this._meta = [];
};

AU.amazonS3Extender.fileSettings.prototype = {
    __class: true,
    _prop: prop,

    acl: function (value) {
        return this._prop('acl', value, arguments.length);
    },
    key: function (value) {
        return this._prop('key', value, arguments.length);
    },
    policy: function (value) {
        return this._prop('policy', value, arguments.length);
    },
    signature: function (value) {
        return this._prop('signature', value, arguments.length);
    },
    contentType: function (value) {
        return this._prop('contentType', value, arguments.length);
    },
    storageClass: function (value) {
        return this._prop('storageClass', value, arguments.length);
    },
    cacheControl: function (value) {
        return this._prop('cacheControl', value, arguments.length);
    },
    expires: function (value) {
        return this._prop('expires', value, arguments.length);
    },
    meta: function (value) {
        ///	<summary>
        ///		Get or set array of additional fields to send along with file.
        ///     Note, that this fields should be added to the policy.
        ///	</summary>
        ///	<param name="value" type="Array">
        ///		Array of additional fields. 
        ///     Additional field can be custom field, like { name: 'author', value: 'John Doe' },
        ///     or it can be predefined field, like { name: 'width', field: 'Width_[itemIndex]'}.
        ///	</param>
        ///	<returns type="Array" />
        return this._prop('meta', value, arguments.length);
    }
};

AU.amazonS3Extender.fileSettings.prototype.constructor = AU.amazonS3Extender.fileSettings;


if(!window.$au && typeof $au != "undefined") window.$au = $au;

if(window.$au) window.$au.imageUploaderFlash = $au.imageUploaderFlash;
	
})(window);