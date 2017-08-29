package com.ramanco.samandroid.api.dtos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class TemplateDto {

    @SerializedName("ID")
    @Expose
    int id;

    @SerializedName("Name")
    @Expose
    String name;

    @SerializedName("Order")
    @Expose
    int order;

    @SerializedName("TemplateCategoryID")
    @Expose
    int templateCategoryID;

    @SerializedName("TemplateCategoryName")
    @Expose
    String templateCategoryName;

    @SerializedName("BackgroundImageID")
    @Expose
    String backgroundImageID;

    @SerializedName("BackgroundImageBase64")
    @Expose
    String backgroundImageBase64;

    @SerializedName("Text")
    @Expose
    String text;

    @SerializedName("Price")
    @Expose
    double price;

    @SerializedName("WidthRatio")
    @Expose
    int widthRatio;

    @SerializedName("HeightRatio")
    @Expose
    int heightRatio;

    @SerializedName("IsActive")
    @Expose
    boolean isActive;

    @SerializedName("CreationTime")
    @Expose
    String creationTime;

    @SerializedName("Creator")
    @Expose
    String creator;

    @SerializedName("LastUpdateTime")
    @Expose
    String lastUpdateTime;

    @SerializedName("TemplateFields")
    @Expose
    TemplateFieldDto[] templateFields;

    @SerializedName("Category")
    @Expose
    TemplateCategoryDto category;

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

    public int getOrder() {
        return order;
    }

    public void setOrder(int order) {
        this.order = order;
    }

    public int getTemplateCategoryID() {
        return templateCategoryID;
    }

    public void setTemplateCategoryID(int templateCategoryID) {
        this.templateCategoryID = templateCategoryID;
    }

    public String getTemplateCategoryName() {
        return templateCategoryName;
    }

    public void setTemplateCategoryName(String templateCategoryName) {
        this.templateCategoryName = templateCategoryName;
    }

    public String getBackgroundImageID() {
        return backgroundImageID;
    }

    public void setBackgroundImageID(String backgroundImageID) {
        this.backgroundImageID = backgroundImageID;
    }

    public String getBackgroundImageBase64() {
        return backgroundImageBase64;
    }

    public void setBackgroundImageBase64(String backgroundImageBase64) {
        this.backgroundImageBase64 = backgroundImageBase64;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }

    public double getPrice() {
        return price;
    }

    public void setPrice(double price) {
        this.price = price;
    }

    public int getWidthRatio() {
        return widthRatio;
    }

    public void setWidthRatio(int widthRatio) {
        this.widthRatio = widthRatio;
    }

    public int getHeightRatio() {
        return heightRatio;
    }

    public void setHeightRatio(int heightRatio) {
        this.heightRatio = heightRatio;
    }

    public boolean isActive() {
        return isActive;
    }

    public void setActive(boolean active) {
        isActive = active;
    }

    public String getCreationTime() {
        return creationTime;
    }

    public void setCreationTime(String creationTime) {
        this.creationTime = creationTime;
    }

    public String getCreator() {
        return creator;
    }

    public void setCreator(String creator) {
        this.creator = creator;
    }

    public String getLastUpdateTime() {
        return lastUpdateTime;
    }

    public void setLastUpdateTime(String lastUpdateTime) {
        this.lastUpdateTime = lastUpdateTime;
    }

    public TemplateFieldDto[] getTemplateFields() {
        return templateFields;
    }

    public void setTemplateFields(TemplateFieldDto[] templateFields) {
        this.templateFields = templateFields;
    }

    public TemplateCategoryDto getCategory() {
        return category;
    }

    public void setCategory(TemplateCategoryDto category) {
        this.category = category;
    }

    //endregion

}
