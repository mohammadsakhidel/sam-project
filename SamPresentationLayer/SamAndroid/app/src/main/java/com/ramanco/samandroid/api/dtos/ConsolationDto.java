package com.ramanco.samandroid.api.dtos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class ConsolationDto {

    //region Fields:
    @Expose
    @SerializedName("ID")
    int id;

    @Expose
    @SerializedName("ObitID")
    int obitID;

    @Expose
    @SerializedName("CustomerID")
    int customerID;

    @Expose
    @SerializedName("TemplateID")
    int templateID;

    @Expose
    @SerializedName("TemplateInfo")
    String templateInfo;

    @Expose
    @SerializedName("Audience")
    String audience;

    @Expose
    @SerializedName("From")
    String from;

    @Expose
    @SerializedName("Status")
    String status;

    @Expose
    @SerializedName("PaymentStatus")
    String paymentStatus;

    @Expose
    @SerializedName("CreationTime")
    String creationTime;

    @Expose
    @SerializedName("LastUpString")
    String lastUpString;

    @Expose
    @SerializedName("TrackingNumber")
    String trackingNumber;
    //endregion

    //region navigation:
    @Expose
    @SerializedName("Customer")
    CustomerDto customer;

    @Expose
    @SerializedName("Obit")
    ObitDto obit;

    @Expose
    @SerializedName("Template")
    TemplateDto template;
    //endregion

    //region Props:
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

    public int getCustomerID() {
        return customerID;
    }

    public void setCustomerID(int customerID) {
        this.customerID = customerID;
    }

    public int getTemplateID() {
        return templateID;
    }

    public void setTemplateID(int templateID) {
        this.templateID = templateID;
    }

    public String getTemplateInfo() {
        return templateInfo;
    }

    public void setTemplateInfo(String templateInfo) {
        this.templateInfo = templateInfo;
    }

    public String getAudience() {
        return audience;
    }

    public void setAudience(String audience) {
        this.audience = audience;
    }

    public String getFrom() {
        return from;
    }

    public void setFrom(String from) {
        this.from = from;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getPaymentStatus() {
        return paymentStatus;
    }

    public void setPaymentStatus(String paymentStatus) {
        this.paymentStatus = paymentStatus;
    }

    public String getCreationTime() {
        return creationTime;
    }

    public void setCreationTime(String creationTime) {
        this.creationTime = creationTime;
    }

    public String getLastUpString() {
        return lastUpString;
    }

    public void setLastUpString(String lastUpString) {
        this.lastUpString = lastUpString;
    }

    public String getTrackingNumber() {
        return trackingNumber;
    }

    public void setTrackingNumber(String trackingNumber) {
        this.trackingNumber = trackingNumber;
    }

    public CustomerDto getCustomer() {
        return customer;
    }

    public void setCustomer(CustomerDto customer) {
        this.customer = customer;
    }

    public ObitDto getObit() {
        return obit;
    }

    public void setObit(ObitDto obit) {
        this.obit = obit;
    }

    public TemplateDto getTemplate() {
        return template;
    }

    public void setTemplate(TemplateDto template) {
        this.template = template;
    }

    //endregion
    
}
