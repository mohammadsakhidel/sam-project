package com.ramanco.samandroid.api.dtos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class ObitDto {

    //region Fields:
    @SerializedName("ID")
    @Expose
    int id;

    @SerializedName("Title")
    @Expose
    String title;

    @SerializedName("ObitType")
    @Expose
    String obitType;

    @SerializedName("MosqueID")
    @Expose
    int mosqueID;

    @SerializedName("CreationTime")
    @Expose
    String creationTime;

    @SerializedName("LastUpdateTime")
    @Expose
    String lastUpdateTime;
    //endregion

    //region Getters & Setters:

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getObitType() {
        return obitType;
    }

    public void setObitType(String obitType) {
        this.obitType = obitType;
    }

    public int getMosqueID() {
        return mosqueID;
    }

    public void setMosqueID(int mosqueID) {
        this.mosqueID = mosqueID;
    }

    public String getCreationTime() {
        return creationTime;
    }

    public void setCreationTime(String creationTime) {
        this.creationTime = creationTime;
    }

    public String getLastUpdateTime() {
        return lastUpdateTime;
    }

    public void setLastUpdateTime(String lastUpdateTime) {
        this.lastUpdateTime = lastUpdateTime;
    }

    //endregion

}
