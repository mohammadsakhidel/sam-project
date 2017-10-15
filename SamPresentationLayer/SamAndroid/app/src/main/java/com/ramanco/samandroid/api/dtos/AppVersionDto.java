package com.ramanco.samandroid.api.dtos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class AppVersionDto {

    //region Fields:
    @Expose
    @SerializedName("VersionName")
    String appVersionName;

    @Expose
    @SerializedName("VersionCode")
    int appVersionCode;

    @Expose
    @SerializedName("NewVersionUrl")
    String newVersionUrl;
    //endregion

    //region Props:
    public String getAppVersionName() {
        return appVersionName;
    }

    public void setAppVersionName(String appVersionName) {
        this.appVersionName = appVersionName;
    }

    public int getAppVersionCode() {
        return appVersionCode;
    }

    public void setAppVersionCode(int appVersionCode) {
        this.appVersionCode = appVersionCode;
    }

    public String getNewVersionUrl() {
        return newVersionUrl;
    }

    public void setNewVersionUrl(String newVersionUrl) {
        this.newVersionUrl = newVersionUrl;
    }

    //endregion

}
