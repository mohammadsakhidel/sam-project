package com.ramanco.samandroid.api.dtos;

import android.support.annotation.NonNull;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import org.joda.time.DateTime;

import java.util.Date;

public class ObitHoldingDto implements Comparable<ObitHoldingDto> {
    
    //region Fields:
    @SerializedName("ID")
    @Expose
    private int id;

    @SerializedName("ObitID")
    @Expose
    private int obitID;

    @SerializedName("BeginTime")
    @Expose
    private String beginTime;

    @SerializedName("EndTime")
    @Expose
    private String endTime;

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

    public String getBeginTime() {
        return beginTime;
    }

    public void setBeginTime(String beginTime) {
        this.beginTime = beginTime;
    }

    public String getEndTime() {
        return endTime;
    }

    public void setEndTime(String endTime) {
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

    //region Additional Fields:
    public DateTime getBeginTimeObject()
    {
        return DateTime.parse(this.beginTime);
    }

    public DateTime getEndTimeObject()
    {
        return DateTime.parse(this.endTime);
    }
    //endregion

    //region Comparable Implementation:
    @Override
    public int compareTo(@NonNull ObitHoldingDto o) {
        return this.getBeginTimeObject().compareTo(o.getBeginTimeObject());
    }
    //endregion
    
}
