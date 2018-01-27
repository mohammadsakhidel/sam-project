package com.ramanco.samandroid.api.dtos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.List;

public class ObitDto {

    //region Fields:
    @SerializedName("ID")
    @Expose
    private int id;

    @SerializedName("Title")
    @Expose
    private String title;

    @SerializedName("ObitType")
    @Expose
    private String obitType;

    @SerializedName("DeceasedIdentifier")
    @Expose
    private String deceasedIdentifier;

    @SerializedName("MosqueID")
    @Expose
    private int mosqueID;

    @SerializedName("CreationTime")
    @Expose
    private String creationTime;

    @SerializedName("LastUpdateTime")
    @Expose
    private String lastUpdateTime;
    //endregion

    //region Navigation Fields:
    @SerializedName("ObitHoldings")
    @Expose
    private ObitHoldingDto[] obitHoldings;
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

    public String getDeceasedIdentifier() {
        return deceasedIdentifier;
    }

    public void setDeceasedIdentifier(String deceasedIdentifier) {
        this.deceasedIdentifier = deceasedIdentifier;
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

    public ObitHoldingDto[] getObitHoldings() {
        return obitHoldings;
    }

    public void setObitHoldings(ObitHoldingDto[] obitHoldings) {
        this.obitHoldings = obitHoldings;
    }

    //endregion

}
