package com.ramanco.samandroid.api.dtos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class CustomerDto {

    @Expose
    @SerializedName("ID")
    public int id;

    @Expose
    @SerializedName("FullName")
    public String fullName;

    @Expose
    @SerializedName("Gender")
    public boolean gender;

    @Expose
    @SerializedName("IsMember")
    public boolean isMember;

    @Expose
    @SerializedName("UserName")
    public String userName;

    @Expose
    @SerializedName("RegistrationTime")
    public String registrationTime;

    @Expose
    @SerializedName("CellPhoneNumber")
    public String cellPhoneNumber;

    //region Getters & Setters

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getFullName() {
        return fullName;
    }

    public void setFullName(String fullName) {
        this.fullName = fullName;
    }

    public boolean isGender() {
        return gender;
    }

    public void setGender(boolean gender) {
        this.gender = gender;
    }

    public boolean isMember() {
        return isMember;
    }

    public void setMember(boolean member) {
        isMember = member;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public String getRegistrationTime() {
        return registrationTime;
    }

    public void setRegistrationTime(String registrationTime) {
        this.registrationTime = registrationTime;
    }

    public String getCellPhoneNumber() {
        return cellPhoneNumber;
    }

    public void setCellPhoneNumber(String cellPhoneNumber) {
        this.cellPhoneNumber = cellPhoneNumber;
    }

    //endregion

}
