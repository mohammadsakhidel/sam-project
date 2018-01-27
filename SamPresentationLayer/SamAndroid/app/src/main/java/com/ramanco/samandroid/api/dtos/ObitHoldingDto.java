package com.ramanco.samandroid.api.dtos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.Date;

public class ObitHoldingDto {
    
    //region Fields:
    @SerializedName("ID")
    @Expose
    private int id;

    @SerializedName("ObitID")
    @Expose
    private int obitID;

    @SerializedName("BeginTime")
    @Expose
    private Date beginTime;

    @SerializedName("EndTime")
    @Expose
    private Date endTime;

    @SerializedName("SaloonID")
    @Expose
    private String saloonId;

    @SerializedName("SaloonName")
    @Expose
    private String saloonName;
    //endregion

    //region Getters & Setters:

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getObitID() {
        return obitID;
    }

    public void setObitID(int obitID) {
        this.obitID = obitID;
    }

    public Date getBeginTime() {
        return beginTime;
    }

    public void setBeginTime(Date beginTime) {
        this.beginTime = beginTime;
    }

    public Date getEndTime() {
        return endTime;
    }

    public void setEndTime(Date endTime) {
        this.endTime = endTime;
    }

    public String getSaloonId() {
        return saloonId;
    }

    public void setSaloonId(String saloonId) {
        this.saloonId = saloonId;
    }

    public String getSaloonName() {
        return saloonName;
    }

    public void setSaloonName(String saloonName) {
        this.saloonName = saloonName;
    }

    //endregion
    
}
