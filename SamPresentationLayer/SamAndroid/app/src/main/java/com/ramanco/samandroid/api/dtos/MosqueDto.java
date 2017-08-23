package com.ramanco.samandroid.api.dtos;

import android.support.annotation.Nullable;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import org.joda.time.DateTime;

public class MosqueDto {
    
    //region fields:
    @SerializedName("ID")
    @Expose
    int id;

    @SerializedName("Name")
    @Expose
    String name;

    @SerializedName("ImamName")
    @Expose
    String imamName;

    @SerializedName("ImamCellPhone")
    @Expose
    String imamCellPhone;

    @SerializedName("InterfaceName")
    @Expose
    String interfaceName;

    @SerializedName("InterfaceCellPhone")
    @Expose
    String interfaceCellPhone;

    @SerializedName("CityID")
    @Expose
    int cityID;

    @SerializedName("Address")
    @Expose
    String address;

    @SerializedName("Location")
    @Expose
    String location;

    @SerializedName("PhoneNumber")
    @Expose
    String phoneNumber;

    @SerializedName("CreationTime")
    @Expose
    String creationTime;

    @SerializedName("LastUpdateTime")
    @Expose
    String lastUpdateTime;

    @SerializedName("Creator")
    @Expose
    String creator;
    //endregion

    //region Getters & Setters:

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getImamName() {
        return imamName;
    }

    public void setImamName(String imamName) {
        this.imamName = imamName;
    }

    public String getImamCellPhone() {
        return imamCellPhone;
    }

    public void setImamCellPhone(String imamCellPhone) {
        this.imamCellPhone = imamCellPhone;
    }

    public String getInterfaceName() {
        return interfaceName;
    }

    public void setInterfaceName(String interfaceName) {
        this.interfaceName = interfaceName;
    }

    public String getInterfaceCellPhone() {
        return interfaceCellPhone;
    }

    public void setInterfaceCellPhone(String interfaceCellPhone) {
        this.interfaceCellPhone = interfaceCellPhone;
    }

    public int getCityID() {
        return cityID;
    }

    public void setCityID(int cityID) {
        this.cityID = cityID;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public String getPhoneNumber() {
        return phoneNumber;
    }

    public void setPhoneNumber(String phoneNumber) {
        this.phoneNumber = phoneNumber;
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

    public String getCreator() {
        return creator;
    }

    public void setCreator(String creator) {
        this.creator = creator;
    }

    //endregion
    
}
