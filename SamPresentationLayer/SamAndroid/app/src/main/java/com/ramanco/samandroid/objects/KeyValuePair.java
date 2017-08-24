package com.ramanco.samandroid.objects;

import android.support.annotation.NonNull;

public class KeyValuePair implements Comparable<KeyValuePair> {
    private String key;
    private String value;
    private String desc;
    private Object tag;

    public KeyValuePair(String key, String value) {
        this.key = key;
        this.value = value;
    }

    //region Getters & Setters
    public String getKey() {
        return key;
    }

    public void setKey(String key) {
        this.key = key;
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }

    public String getDesc() {
        return desc;
    }

    public void setDesc(String desc) {
        this.desc = desc;
    }

    public Object getTag() {
        return tag;
    }

    public void setTag(Object tag) {
        this.tag = tag;
    }

    //endregion

    @Override
    public int compareTo(@NonNull KeyValuePair o) {
        return this.getValue().compareTo(o.getValue());
    }
}
